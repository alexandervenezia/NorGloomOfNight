using Godot;
using System;

public partial class FPS : Label
{
    public override void _Process(double delta)
    {
        if (OS.IsDebugBuild())
            Text = "FPS: " + Engine.GetFramesPerSecond() + "\n" + "RAM: " + (int)(OS.GetStaticMemoryUsage() * 0.000001) + "MB";
        else
            Visible = false;

    }
}
