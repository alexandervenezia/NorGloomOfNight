using Godot;
using System;

public partial class Background : ParallaxBackground
{
    private Sprite2D _sprite;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Bg");
    }

    public void SetBG(Texture2D bg)
    {
        GD.Print("BG: " + bg);
        _sprite.Texture = bg;
    }
}
