using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class CutscenePlayer : ColorRect
{
    private Camera2D _camera;
    private Random RNG;
    private int _lineIndex;
    private Cutscene _data;
    private RichTextLabel _dialogue;
    private AudioStreamPlayer _clickSound;
    private string _dialoguePrefix;
    private bool _rendering = false;
    private bool _panning = false;
    private bool _skipped = false;

    private Vector2I _cameraOffset;
    

    public delegate void CutsceneEndingInformer();
    public event CutsceneEndingInformer OnEnd;


    public override void _Ready()
    {
        _dialogue = GetNode<RichTextLabel>("Dialogue");
        _clickSound = GetNode<AudioStreamPlayer>("Click");
        _dialoguePrefix = _dialogue.Text;
        _lineIndex = 0;
        Visible = false;
        RNG = new();

        _camera = GetNodeOrNull<Camera2D>("..");
        if (_camera == null)
            GD.Print("WARNING: _camera of CutscenePlayer is null; panning not supported");
    }

    public void SetCutscene(Cutscene cutscene)
    {
        Visible = true;
        _lineIndex = 0;
        _data = cutscene;
        Engine.TimeScale = 0f;
        LoadNextLine();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Select") || Input.IsActionJustPressed("Jump"))
        {
            if (_data != null && !_panning)
            {
                if (_rendering)
                {
                    _skipped = true;
                }
                if (!_skipped)
                    LoadNextLine();
            }
        }
    }

    private void LoadNextLine()
    {
        if (_lineIndex >= _data.Lines.Length)
        {
            Engine.TimeScale = 1f;
            Visible = false;
            _data = null;
            OnEnd?.Invoke();
            return;
        }
        string fullText;

        if (_data.Lines[_lineIndex].StartsWith("<pan"))
        {
            HandlePan(_data.Lines[_lineIndex]);
            _lineIndex++;
            return;
        }

        if (_data.SpeakerName != "")
            fullText = _dialoguePrefix + ParseLine(_data.SpeakerName + ":<br>[/color]" + _data.Lines[_lineIndex]);
        else
            fullText = _dialoguePrefix + ParseLine(_data.Lines[_lineIndex]);

        // NOTE: This disables the "Speaker's name" component
        fullText = _dialoguePrefix + ParseLine(_data.Lines[_lineIndex]);        

        _lineIndex++;
        _rendering = true;
        RenderTextAnimation(fullText);
    }

    private string ParseLine(string raw)
    {
        string parsed = "";

        parsed = raw.Replace("<narration>", "[i][color=#444444]");
        parsed = parsed.Replace("</narration>", "[/color][/i]");
        parsed = parsed.Replace("<br>", "\n    ");

        return parsed;
    }

    private async void HandlePan(string command)
    {
        _panning = true;
        string parsed = command;
        parsed = parsed.Replace("<", "");
        parsed = parsed.Replace(">", "");
        parsed = parsed.Split("=")[1].StripEdges();

        int x, y;
        float duration;

        string[] coords = parsed.Split("x");

        if (coords[0].StartsWith("return"))
        {
            x = 0; //-_cameraOffset.X;
            y = 0; //-_cameraOffset.Y;
            duration = coords[1].ToFloat();
        }
        else
        {
            x = coords[0].ToInt() + (int)_camera.Position.X;
            y = coords[1].ToInt() + (int)_camera.Position.Y;
            duration = coords[2].ToFloat();
        }

        Tween pan = GetTree().CreateTween();
        pan.SetParallel(true);
        //pan.SetSpeedScale(1f);
        pan.TweenProperty(_camera, "position:x", x, duration);
        pan.TweenProperty(_camera, "position:y", y, duration);
        pan.Play();

        GD.Print(x, "; ", y, "; ", duration);

        while (pan.CustomStep(0.016))
        {            
            await Task.Delay(16);
        }

        _cameraOffset = _cameraOffset + new Vector2I(x, y);

        _panning = false;
        LoadNextLine();
    }

    private async void RenderTextAnimation(string text)
    {
        _dialogue.Text = "";
        int charIndex = 0;

        while (charIndex < text.Length)
        {
            if (_skipped)
            {
                _dialogue.Text = text;
                _rendering = false;
                _skipped = false;
                return;
            }

            if (text[charIndex] == '[')
            {
                string addendum = "[";
                charIndex++;
                while (text[charIndex] != ']')
                {
                    addendum += text[charIndex];
                    charIndex++;
                }
                _dialogue.Text = _dialogue.Text + addendum + ']';
                charIndex++;
            }
            else
            {
                int delay = 30;
                _dialogue.Text = _dialogue.Text + text[charIndex];
                if (text[charIndex] == '\n')
                {
                    delay = 500;
                }
                else if (text[charIndex] == '.' || text[charIndex] == '—')
                    delay = 240;
                else if (text[charIndex] == ',')
                    delay = 150;
                charIndex++;
                _clickSound.PitchScale = (float)(1.1 - RNG.NextDouble() * 0.2);
                if (!_clickSound.Playing)
                    _clickSound.Play();
                await Task.Delay(delay);
            }
        }

        _rendering = false;

    }
}
