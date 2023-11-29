using Godot;
using Overworld;
using System;

public partial class ProgressBlocker : StaticBody2D
{
    public override void _Ready()
    {
        QuestManager.GetInstance().FlagChanged += OnFlagChange;
    }

    private void OnFlagChange()
    {
        if (QuestManager.GetInstance().FLAG_TALKED_TO_MANAGER && IsInstanceValid(this))
        {
            GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        }
    }
}
