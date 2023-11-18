using Godot;
using Overworld;
using System;

public partial class HellLevel : Node2D, ILevel
{
	[Export] protected Vector2 _playerSpawn;
	protected Player _player;
	protected Player Player => _player;
	protected ICombatable _enemyInCombat;
	public override void _Ready()
	{
		GD.Print("Hello, Hell.");       
	}

	protected async void OnAggro(ICombatable enemy)
	{
		((Node)enemy).CallDeferred("Disable");
		_enemyInCombat = enemy;
		GD.Print("Aggroed");
		// Either begin combat immediately or after cutscene
		// TODO: Begin combat here
		MasterScene master = MasterScene.GetInstance();
		master.SetEnemyIDs(enemy.GetEnemyIDs());
		master.SetPlayerHP(_player.CurrentHealth);
		master.CallDeferred("ActivateScene", master.CombatSceneUID, true, true);
		
	}

	public Player GetPlayer()
	{
		return _player;
	}

	public void SetPlayer(Player player)
	{
		_player = player;
		_player.EnemyAggroed += OnAggro;
		_player.Position = _playerSpawn;
	}

	public void Reactivate()
	{
		_player?.SetHealth(MasterScene.GetInstance().LoadPlayerHP());
		if (_player != null && _player.CurrentHealth <= 0)
		{
			_enemyInCombat?.Enable();
			_player.GlobalPosition = _playerSpawn;
			_player.SetHealth(_player.MaxHealth);
		}
		else
		{
			if (_enemyInCombat != null)
				_enemyInCombat.Die();
		}
	}

	public void UseElevator()
	{
		
	}
}
