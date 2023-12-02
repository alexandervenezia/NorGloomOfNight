namespace Overworld;

using Godot;
using Overworld;
using System;
using System.Security.AccessControl;
using System.Threading.Tasks;


public partial class Purgatory : Node2D, ILevel
{
	[Export] private string _managementUID;
	[Export] private string _prideUID;
	[Export] private string _journalID; // No longer used
	[Export] private string _introMusicUID;
	[Export] private string _loopMusicUID;
	
	private Player _player;
	public Player Player => _player;
	private Vector2 _playerSpawn;

	private ICombatable _enemyInCombat;

	private Journal _journal;

	public override void _Ready()
	{
		GD.Print("Initial audio");
		MasterAudio.GetInstance().ClearQueue();
		MasterAudio.GetInstance().PlaySong(_introMusicUID);
		MasterAudio.GetInstance().QueueSong(_loopMusicUID);

		_player = (Player)GetNodeOrNull("OverworldPlayer");
		_journal = _player.GetNode<Camera2D>("Camera2D").GetNode<Journal>("Journal");
		_journal.Visible = false;

		if (_player != null)
			_player.EnemyAggroed += OnAggro;
		_playerSpawn = _player.GlobalPosition;
		_playerSpawn += Vector2.Right * 1675; // Hardcoded elevator offset; dumb
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
		MasterScene master = MasterScene.GetInstance();
		master.SetIsBoss(enemy.IsBoss());
		master.SetEnemyIDs(enemy.GetEnemyIDs());
		master.SetPlayerHP(_player.CurrentHealth);		

		GD.Print("PlayerPos: ", _player.Position);
		GD.Print("PlayerVel: ", _player.Velocity);
		GD.Print("PlayerRealVel: ", _player.GetRealVelocity());
		
		Vector2 storedPlayerPos = _player.Position;
		Engine.TimeScale = 0f;
		_player.SetPhysicsProcess(false);

		Tween introTween = GetTree().CreateTween();
		Vector2 oldZoom = _player.GetNode<Camera2D>("Camera2D").Zoom;
		GD.Print(oldZoom);
		introTween.TweenProperty(_player.GetNode<Camera2D>("Camera2D"), "zoom", new Vector2(1f, 1f), 1.0f).SetTrans(Tween.TransitionType.Circ);

		_player.EncounterSong.Play();

		while (introTween.CustomStep(0.016))
		{
			await Task.Delay(16);
			_player.Position = storedPlayerPos;
			_player.Velocity = Vector2.Zero;
		}
		
		GD.Print("PlayerPos: ", _player.Position);
		GD.Print("PlayerVel: ", _player.Velocity);
		GD.Print("PlayerRealVel: ", _player.GetRealVelocity());
		await Task.Delay(500);
		GD.Print("PlayerPos: ", _player.Position);
		GD.Print("PlayerVel: ", _player.Velocity);
		GD.Print("PlayerRealVel: ", _player.GetRealVelocity());
		_player.Position = storedPlayerPos;
		_player.Velocity = Vector2.Zero;
		GD.Print("PlayerPos: ", _player.Position);
		GD.Print("PlayerVel: ", _player.Velocity);
		GD.Print("PlayerRealVel: ", _player.GetRealVelocity());

		_player.GetNode<Camera2D>("Camera2D").Zoom = oldZoom;
		Engine.TimeScale = 1f;
		GD.Print("PlayerPos: ", _player.Position);
		GD.Print("PlayerVel: ", _player.Velocity);
		GD.Print("PlayerRealVel: ", _player.GetRealVelocity());

		_player.SetPhysicsProcess(true);
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

	public override void _Process(double delta)
	{
		if (OS.IsDebugBuild() && Input.IsKeyPressed(Key.R))
		{
			Node pride = MasterScene.GetInstance().ActivateScene(_prideUID, true, true);
			_player.EnemyAggroed -= OnAggro;
			RemoveChild(_player);
			pride.AddChild(_player);
			((Pride)pride).SetPlayer(_player);
		}

		if (OS.IsDebugBuild() && Input.IsKeyLabelPressed(Key.T))
		{
			QuestManager.GetInstance().FLAG_TALKED_TO_MANAGER = true;
			QuestManager.GetInstance().FLAG_ACQUIRED_CROWN = true;
		}

		if (Input.IsActionJustPressed("Escape") && !_player.PlayingCutscene)
		{
			if (!_journal.Visible)
				_journal.Open();				
			else
				_journal.Close();
		}
	}

	public void Reactivate()
	{		
		GD.Print("Reactivating Purgatory");
		if (!MasterAudio.GetInstance().GetNoRestart())
		{
			GD.Print("Audio should play");
			MasterAudio.GetInstance().ClearQueue();
			MasterAudio.GetInstance().PlaySong(_introMusicUID);
			MasterAudio.GetInstance().QueueSong(_loopMusicUID);
		}

		GD.Print("Enemy: " + _enemyInCombat);
		if (!IsInstanceValid((Node)_enemyInCombat))
			_enemyInCombat = null;

		_player?.SetHealth(MasterScene.GetInstance().LoadPlayerHP());
		_player?.AddCoins(MasterScene.GetInstance().CollectCoins());

		if (_player.CurrentHealth == -1)
			_player.FullHeal();

		if (_player != null && _player.CurrentHealth <= 0)
		{
			GD.Print("Respawn: " + _player.CurrentHealth);
			_enemyInCombat?.Enable();
			_player.GlobalPosition = _playerSpawn;
			_player.SetHealth(_player.MaxHealth);

			_player.Die();
		}
		else
		{
			_enemyInCombat?.Die();
			_enemyInCombat = null;
		}
	}

	public Node UseElevator(string dest = "")
	{
		//MasterScene.GetInstance().SetPlayerHP(_player.CurrentHealth);

		MasterScene.GetInstance().SetPlayerHP(_player.CurrentHealth);
		MasterScene.GetInstance().TotalCoins = _player.Coins;
		MasterScene.GetInstance().SetCombatBackground(0);

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
