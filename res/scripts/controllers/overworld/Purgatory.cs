using Godot;
using Overworld;
using System;
using System.Security.AccessControl;

public partial class Purgatory : Node2D, ILevel
{
	[Export] private string _managementUID;
	[Export] private string _prideUID;
	private Player _player;
	public Player Player => _player;
	private Vector2 _playerSpawn;

	private ICombatable _enemyInCombat;

	public override void _Ready()
	{
		_player = (Player)GetNodeOrNull("OverworldPlayer");
		if (_player != null)
			_player.EnemyAggroed += OnAggro;
		_playerSpawn = _player.GlobalPosition;
	}

	private async void OnAggro(ICombatable enemy)
	{
		GD.Print(enemy.IsEnabled());

		if (enemy.HasIntroCutscene())
		{
			await _player.PlayCutscene(enemy.GetIntroCutscene());
		}

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
		GD.Print("SetPLayer");
		_player = player;
		_player.EnemyAggroed += OnAggro;
		_player.GlobalPosition = _playerSpawn;
	}

	// TESTING TOOL
	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.Delete))
		{
			Node pride = MasterScene.GetInstance().ActivateScene(_prideUID, true, true);
			_player.EnemyAggroed -= OnAggro;
			RemoveChild(_player);
			pride.AddChild(_player);
			((Pride)pride).SetPlayer(_player);
		}
	}

	public void Reactivate()
	{
		GD.Print("Enemy: " + _enemyInCombat);
		_player?.SetHealth(MasterScene.GetInstance().LoadPlayerHP());
		if (_player != null && _player.CurrentHealth <= 0)
		{
			GD.Print("Respawn: " + _player.CurrentHealth);
			_enemyInCombat?.Enable();
			_player.GlobalPosition = _playerSpawn;
			_player.SetHealth(_player.MaxHealth);
		}
		else
			_enemyInCombat?.Die();
	}

	public Node UseElevator(string dest="")
	{		
		//MasterScene.GetInstance().SetPlayerHP(_player.CurrentHealth);
		
		MasterScene.GetInstance().SetPlayerHP(_player.CurrentHealth);
		
		Node destination = MasterScene.GetInstance().ActivateScene(dest, true, true);
		
		if (destination is HellLevel)
		{
			RemoveChild(_player);
			_player.EnemyAggroed -= OnAggro;
			destination.AddChild(_player);
			SetOwnerRecursive(_player, destination);
			_player.GetNode<Button>("Camera2D/BackToPurgatory/LevelSelectMenu/Purgatory").Owner = destination;
			((HellLevel)destination).SetPlayer(_player);

			
		}

		return destination;
	}

	private void SetOwnerRecursive(Node root, Node owner)
	{
		foreach (Node n in root.GetChildren())
		{
			n.Owner = owner;
			SetOwnerRecursive(n, owner);
		}
	}

}
