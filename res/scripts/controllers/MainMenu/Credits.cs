using Godot;
using System;

public partial class Credits : Node2D
{
    private const float SCROLL_SPEED = -120f;

    private ColorRect _credits;
    private float _progress = 0;
    public override void _Ready()
    {
        _progress = -550;
        _credits = GetNode<ColorRect>("ColorRect");
        GD.Print("Time scale: ", Engine.TimeScale);
    }
    public override void _Process(double delta)
    {
        _progress += (float)(delta * SCROLL_SPEED);
        if (_progress > -15500)
            _credits.Position = new Vector2(_credits.Position.X, _progress);
    }
}
