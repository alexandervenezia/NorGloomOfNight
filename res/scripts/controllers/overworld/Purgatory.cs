using Godot;
using Overworld;
using System;

public partial class Purgatory : Node2D, ILevel
{
    private Player _player;
    public Player Player => _player;
    public override void _Ready()
    {
        _player = (Player)GetNode("OverworldPlayer");
    }

    public Player GetPlayer()
    {
        return _player;
    }
}
