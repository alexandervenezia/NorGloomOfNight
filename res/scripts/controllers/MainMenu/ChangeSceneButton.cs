using Godot;
using System;
using System.Threading.Tasks;

public partial class ChangeSceneButton : Button
{
    [Export] private string _destinationScene;
    [Export] private bool _loadLastScene;
    [Export] private bool _reloadMasterScene; // HORRIBLE

    public override void _Ready()
    {
        Pressed += OnPressed;
    }

    private async void OnPressed()
    {
        if (_reloadMasterScene)
        {
            MasterScene oldMaster = MasterScene.GetInstance();
            MasterScene newMaster = GD.Load<PackedScene>(_destinationScene).Instantiate<MasterScene>();

            await Task.Delay(500);
            GD.Print("Assigning");

            GetTree().Root.AddChild(newMaster);
            oldMaster.Free();
            
            return;
            
        }
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
