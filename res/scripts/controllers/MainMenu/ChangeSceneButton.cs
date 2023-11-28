using Godot;
using System;

public partial class ChangeSceneButton : Button
{
    [Export] private string _destinationScene;

    public override void _Ready()
    {
        Pressed += OnPressed;
    }

    private void OnPressed()
    {
        MasterScene.GetInstance().ActivateScene(_destinationScene, true, true);
    }
}
