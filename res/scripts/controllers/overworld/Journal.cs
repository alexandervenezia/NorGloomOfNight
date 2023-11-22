using Godot;
using System;

public partial class Journal : Node2D
{
    public override void _Ready()
    {
		MasterAudio.GetInstance().SetNoRestart();
    }
    public override void _Process(double delta)
    {
        
    }
}
