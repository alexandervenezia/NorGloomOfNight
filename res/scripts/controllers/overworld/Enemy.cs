namespace Overworld;

using Godot;
using System;
using System.Diagnostics;
using System.Linq;



public partial class Enemy : CharacterBody2D
{
    [Export] protected string _name;
    [Export] protected string _uid; // Must match UID of combat equivalent
    [Export] protected float _speed;
    [Export] protected float _acceleration;

    protected AnimatedSprite2D _sprite;

    

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _Process(double delta)
    {        
        MoveAndSlide();

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            OnCollision(collision);
        }
    }

    protected virtual void OnCollision(KinematicCollision2D collision)
    {

    }

    
}
