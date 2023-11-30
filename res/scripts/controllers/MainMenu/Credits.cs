using Godot;
using System;

public partial class Credits : Node2D
{
    private const int SCROLL_SPEED = -50;

    private ColorRect _credits;
    private int _progress = 0;
    public override void _Ready()
    {
        _progress = 0;
        _credits = GetNode<ColorRect>("ColorRect");
    }
    public override void _Process(double delta)
    {
        //_progress += (int)(delta * SCROLL_SPEED);
        //_credits.Position = new Vector2(0, _progress);
    }
}
