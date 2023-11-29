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
	private static Random RNG = new();
	[Export] protected string _name;
	[Export] protected int _id; // Must match UID of combat equivalent
	[Export] protected float _speed;
	[Export] protected float _acceleration;

	[Export] protected Godot.Collections.Dictionary<int, int> _firstSlotSpawn;
	[Export] protected Godot.Collections.Dictionary<int, int> _secondSlotSpawn;
	[Export] protected Godot.Collections.Dictionary<int, int> _thirdSlotSpawn;
	[Export] protected Godot.Collections.Dictionary<int, int> _fourthSlotSpawn;
	

	protected AnimatedSprite2D _sprite;
	protected Label _warningLabel;
	protected Vector2 _spawnPoint;

	private bool _enabled;
	

	public override void _Ready()
	{
		_enabled = true;
		_spawnPoint = Position;
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_sprite.Play("default");

		_warningLabel = GetNodeOrNull<Label>("Warning");
		if (_warningLabel != null)
			_warningLabel.Visible = true;

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

	private int GetEnemyID(Godot.Collections.Dictionary<int, int> dict)
	{
		if (dict == null || dict.Count == 0)
			return -1;

		int returnID = -1;
		foreach (int id in dict.Keys)
		{
			if (id <= 0)
				continue;
			
			if (RNG.Next(0, 100) < dict[id])
			{
				returnID = id;
				break;
			}
		}

		GD.Print(returnID);

		return returnID;
	}

	public List<int> GetEnemyIDs()
	{
		List<int> enemyIds = new();

		int firstSlot = GetEnemyID(_firstSlotSpawn);
		int secondSlot = GetEnemyID(_secondSlotSpawn);
		int thirdSlot = GetEnemyID(_thirdSlotSpawn);
		int fourthSlot = GetEnemyID(_fourthSlotSpawn);

		if (firstSlot != -1)
			enemyIds.Add(firstSlot);
		if (secondSlot != -1)
			enemyIds.Add(secondSlot);
		if (thirdSlot != -1)
			enemyIds.Add(thirdSlot);
		if (fourthSlot != -1)
			enemyIds.Add(fourthSlot);

		

		return enemyIds;
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
