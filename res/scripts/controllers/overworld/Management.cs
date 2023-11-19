namespace Overworld;

using Godot;
using System;

public partial class Management : Node2D
{
    [Export] private Cutscene _cutscene;
    [Export] private CutscenePlayer _cutscenePlayer;
    [Export] private LevelSelectOption _returnButton;

    public override void _Ready()
    {
        _returnButton.Visible = false;
        _cutscenePlayer.SetCutscene(_cutscene);
        _cutscenePlayer.OnEnd += OnCutsceneEnd;
    }

    private void OnCutsceneEnd()
    {
        _returnButton.Visible = true;
    }
}
