using Godot;
using System;

public partial class CombatInstructions : Sprite2D
{
    private const float SLIDE_TIME = 0.5f;
    private Vector2 _basePos;
    private Vector2 _loweredPos;

    Tween vertTween;

    public override void _Ready()
    {
        _basePos = Position;
        _loweredPos = _basePos + new Vector2(0, 315);
    }
    public void Enable()
    {           
        vertTween = GetTree().CreateTween();
        vertTween.TweenProperty(this, "position:y", _loweredPos.Y, SLIDE_TIME);
        Visible = true;
    }

    public void Disable()
    {
        vertTween = GetTree().CreateTween();
        vertTween.TweenProperty(this, "position:y", _basePos.Y, SLIDE_TIME);
        Visible = false;
    }
}
