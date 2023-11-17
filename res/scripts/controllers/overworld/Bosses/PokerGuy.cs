using Godot;
using System;
using System.Collections.Generic;

public partial class PokerGuy : Node2D, ICombatable
{
    private AnimatedSprite2D _sprite;
    private Area2D _area;   

    public override void _Ready()
    {
        _sprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
        _area = (Area2D)GetNode("Aggro");
        _sprite.Play("idle");
    }

    public List<int> GetEnemyIDs()
    {
        return new List<int>{91, 1}; // TODO:
    }
}
