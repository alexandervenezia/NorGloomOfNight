using Godot;
using System;

public partial class Journal : Node2D
{
    private ChangeSceneButton _returnButton;
    private JournalLine _openDeck;
    private JournalLine _openSettings;
    private JournalLine _exit;

    public override void _Ready()
    {
        GD.Print("Ready");
        MasterAudio.GetInstance().SetNoRestart();

        _returnButton = GetNode<ChangeSceneButton>("BackToWork");
        _returnButton.Pressed += Close;

        _openDeck = GetNode<JournalLine>("ViewDeck");
        _openDeck.OnLineClicked += OpenDeck;

        _openSettings = GetNode<JournalLine>("Settings");
        _openSettings.OnLineClicked += OpenSettings;

        _exit = GetNode<JournalLine>("Exit");
        _exit.OnLineClicked += () => {
            GetTree().Quit();
        };
    }
    public override void _Process(double delta)
    {

    }

    public void Open()
    {
        Visible = true;
        Engine.TimeScale = 0f;
    }
    public void Close()
    {
        Visible = false;
        Engine.TimeScale = 1f;
        // QueueFree();        
    }

    private void OpenDeck()
    {
        GD.Print("Deck opened");
    }

    private void OpenSettings()
    {
        
    }
}
