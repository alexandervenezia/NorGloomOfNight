using Godot;
using System;

public partial class ChangeSceneButton : Button
{
    [Export] private string _destinationScene;
    [Export] private bool _loadLastScene;

    public override void _Ready()
    {
        Pressed += OnPressed;
    }

    private void OnPressed()
    {
        if (_loadLastScene)
        {
            GD.Print("Loading last");
            MasterScene.GetInstance().ActivatePreviousScene(true);
            return;
        }
        if (_destinationScene != null)
        {
            GD.Print("Dest");
            MasterScene.GetInstance().ActivateScene(_destinationScene, true, true);
        }
    }
}
