namespace Overworld;

using Data;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Systems.Combat;



public partial class Enemy : CharacterBody2D, ICombatable
{
	[Export] protected string _name;
	[Export] protected int _id; // Must match UID of combat equivalent
	[Export] protected float _speed;
	[Export] protected float _acceleration;
	

	protected AnimatedSprite2D _sprite;
	protected Vector2 _spawnPoint;

	private bool _enabled;
	

	public override void _Ready()
	{
		_enabled = true;
		_spawnPoint = Position;
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_sprite.Play("default");
	}

	public override void _PhysicsProcess(double delta)
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

	public List<int> GetEnemyIDs()
	{
		return new List<int>{_id};
	}

	public void Disable()
	{
		_enabled = false;
	}

	public void Enable()
	{
		_enabled = true;
	}

	public bool IsEnabled()
	{
		return _enabled;
	}

	public void Die()
	{
		QueueFree();
	}

	public bool HasIntroCutscene()
	{
		return false;
	}

	public Cutscene GetIntroCutscene()
	{
		return null;
	}

	

}
