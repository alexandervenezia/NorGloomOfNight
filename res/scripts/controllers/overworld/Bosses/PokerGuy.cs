using Godot;
using Overworld;
using System;
using System.Collections.Generic;

public partial class PokerGuy : Node2D, ICombatable
{
    private AnimatedSprite2D _sprite;
    private Area2D _area;   

    private bool _enabled;
    [Export] private Cutscene _introCutscene;

    public override void _Ready()
    {
        _enabled = true;
        _sprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
        _area = (Area2D)GetNode("Aggro");
        _sprite.Play("idle");
    }

    public List<int> GetEnemyIDs()
    {
        return new List<int>{91};
    }

    public void Disable()
    {
        _enabled = false;
        _area.Monitorable = false;
    }

    public void Enable()
    {
        _enabled = true;
        _area.Monitorable = true;
    }

    public bool IsEnabled()
    {
        return _enabled;
    }

    public void Die() 
    {
        GD.Print("Poker Guy Died");
        QuestManager.GetInstance().FLAG_ACQUIRED_CROWN = true;
    }

    public bool HasIntroCutscene()
	{
		return true;
	}

	public Cutscene GetIntroCutscene()
	{
		return _introCutscene;
	}

}
