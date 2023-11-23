using Godot;
using Overworld;
using System;
using System.Runtime.CompilerServices;

public partial class HellLevel : Node2D, ILevel
{
	[Export] protected Vector2 _playerSpawn;
	[Export] protected string _journalID;
	[Export] private string _introMusicUID;
	[Export] private string _loopMusicUID;
	protected Player _player;
	protected Player Player => _player;
	protected ICombatable _enemyInCombat;
	public override void _Ready()
	{
		GD.Print("Hello, Hell.");       
	}

	protected async void OnAggro(ICombatable enemy)
	{
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

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Escape"))
		{
			Journal journal = (Journal)UseElevator(_journalID);
		}
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
		GetNode<Elevator>("Elevator").SetLevelSelect(_player.GetNode<Control>("Camera2D/BackToPurgatory"));
	}

	public void Reactivate()
	{
		if (!MasterAudio.GetInstance().GetNoRestart())
		{
			MasterAudio.GetInstance().ClearQueue();
			MasterAudio.GetInstance().PlaySong(_introMusicUID);
			MasterAudio.GetInstance().QueueSong(_loopMusicUID);
		}

		if (!IsInstanceValid((Node)_enemyInCombat))
			_enemyInCombat = null;
			
		_player?.SetHealth(MasterScene.GetInstance().LoadPlayerHP());
		_player?.AddCoins(MasterScene.GetInstance().CollectCoins());
		if (_player != null && _player.CurrentHealth <= 0)
		{
			_enemyInCombat?.Enable();
			_player.GlobalPosition = _playerSpawn;
			_player.SetHealth(_player.MaxHealth);

			_player.Die();
		}
		else
		{
			_enemyInCombat?.Die();
		}
	}

	public Node UseElevator(string dest="")
	{
		GD.Print(dest);
		MasterScene.GetInstance().SetPlayerHP(GetPlayer().CurrentHealth);
        // MasterScene.GetInstance().CallDeferred("ActivatePreviousScene", true);
		Node destination = MasterScene.GetInstance().ActivateScene(dest, true, false);

		if (destination is Purgatory)
		{
			RemoveChild(_player);
			_player.EnemyAggroed -= OnAggro;
			destination.AddChild(_player);
			SetOwnerRecursive(_player, destination);
			((Purgatory)destination).SetPlayer(_player);
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
