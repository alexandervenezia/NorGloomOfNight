namespace Overworld;

using Godot;
using System;

public partial class HealArea : Area2D
{
	private ILevel _level;
	private bool _playerInArea;

	public override void _Ready()
	{
		_level = MasterScene.GetInstance().GetActiveScene<ILevel>();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_playerInArea)
		{
			if (Input.IsActionJustPressed("ui_interact"))
			{
				_level.GetPlayer().FullHeal(true);
			}
		}
	}

	public void Enter()
	{
		_playerInArea = true;
	}

	public void Exit()
	{
		_playerInArea = false;
	}
}
