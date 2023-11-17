using Godot;
using Overworld;
using System;

public partial class Pride : Node2D, ILevel
{
	[Export] private Vector2 _playerSpawn;
	private Player _player;
	public Player Player => _player;
	private ICombatable _enemyInCombat;
	public override void _Ready()
	{
		GD.Print("Hello, Hell.");       
	}

	private async void OnAggro(ICombatable enemy)
	{
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
		_player.GlobalPosition = _playerSpawn;
	}

	public void Reactivate()
	{
		_player?.SetHealth(MasterScene.GetInstance().LoadPlayerHP());
		if (_player != null && _player.CurrentHealth <= 0)
			_enemyInCombat?.Enable();
	}
}
