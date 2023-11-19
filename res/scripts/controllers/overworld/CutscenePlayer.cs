using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class CutscenePlayer : ColorRect
{
    private Random RNG;
    private int _lineIndex;
    private Cutscene _data;
    private RichTextLabel _dialogue;
    private AudioStreamPlayer _clickSound;
    private string _dialoguePrefix;
    private bool _rendering = false;
    private bool _skipped = false;

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
    }

    public void SetCutscene(Cutscene cutscene)
    {
        Visible = true;
        _data = cutscene;
        Engine.TimeScale = 0f;
        LoadNextLine();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Select"))
        {
            if (_data != null)
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
            OnEnd?.Invoke();
            return;
        }
        string fullText = _dialoguePrefix + ParseLine(_data.SpeakerName + ":<br></narration>" + _data.Lines[_lineIndex]);

        _lineIndex++;
        _rendering = true;
        RenderTextAnimation(fullText);
    }

    private string ParseLine(string raw)
    {
        string parsed = "";

        parsed = raw.Replace("<narration>", "[color=#444444]");
        parsed = parsed.Replace("</narration>", "[/color]");
        parsed = parsed.Replace("<br>", "\n    ");

        return parsed;
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
                    delay = 750;
                }
                else if (text[charIndex] == '.' || text[charIndex] == '-')
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
