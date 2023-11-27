namespace Overworld;

using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class NPC : Area2D
{
    [Export] private Godot.Collections.Array<string> _dialogue;
    [Export] private int _delayMS;
    private List<string> _dialogueLines;
    private ILevel _level;
    private Polygon2D _dialogueBubble;
    private RichTextLabel _dialogueBubbleText;
    private bool _playerInArea;
    private bool _dialogueRunning;

    public override void _Ready()
    {
        _level = MasterScene.GetInstance().GetActiveScene<ILevel>();
        _dialogueBubble = GetNode<Polygon2D>("DialogueBubble");
        _dialogueBubbleText = _dialogueBubble.GetNode<RichTextLabel>("RichTextLabel");

        _dialogueLines = new();

        foreach (string l in _dialogue)
        {
            if (l.Length == 0)
                continue;
            _dialogueLines.Add(l);
        }

        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
    }

    public override void _Process(double delta)
    {
        if (_playerInArea && Input.IsActionJustPressed("ui_interact") && !_dialogueRunning)
        {
            GD.Print("Dialogue started");
            StartDialogue();
        }
    }

    public void OnAreaEntered(Area2D area)
    {
        if (area.Owner is Player)
        {
            GD.Print("Entered");
            _playerInArea = true;
        }
    }

    public void OnAreaExited(Area2D area)
    {
        if (area.Owner is Player)
        {
            GD.Print("Exited");
            _playerInArea = false;
        }
    }

    private async void StartDialogue()
    {        
        _dialogueRunning = true;

        foreach (string line in _dialogueLines)
        {
            _dialogueBubbleText.Text = line;
            await Task.Delay(_delayMS);
        }

        _dialogueRunning = false;
        _dialogueBubbleText.Text = "[center]\n\n. . .";
    }


}
