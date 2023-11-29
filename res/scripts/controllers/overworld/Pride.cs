using Godot;
using Overworld;
using System;

public partial class Pride : HellLevel
{
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (OS.IsDebugBuild() && Input.IsKeyPressed(Key.Delete))
        {
            QuestManager.GetInstance().FLAG_TALKED_TO_MANAGER = true;
            QuestManager.GetInstance().FLAG_ACQUIRED_CROWN = true;
            QuestManager.GetInstance().FLAG_BEAT_DOG = true;

            if (Input.IsActionJustPressed("Select"))
            {
                _player.DebugTeleport(_player.GetLocalMousePosition());
            }
        }
    }
}
