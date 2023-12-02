using Godot;
using Overworld;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class HellLevel : Node2D, ILevel
{
	[Export] protected Vector2 _playerSpawn;
	[Export] protected string _journalID; // No longer used
	[Export] protected string _introMusicUID;
	[Export] protected string _loopMusicUID;

	protected Player _player;
	protected Journal _journal;
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
		master.SetCombatBackground(GetBackgroundID());
		master.SetIsBoss(enemy.IsBoss());
		master.SetEnemyIDs(enemy.GetEnemyIDs());
		master.SetPlayerHP(_player.CurrentHealth);
		
		GD.Print("PlayerPos: ", _player.Position);
		GD.Print("PlayerVel: ", _player.Velocity);
		GD.Print("PlayerRealVel: ", _player.GetRealVelocity());
		
		Vector2 storedPlayerPos = _player.Position;;
		Engine.TimeScale = 0f;

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

		
		
		master.CallDeferred("ActivateScene", master.CombatSceneUID, true, true);

	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Escape") && !_player.PlayingCutscene)
		{
			if (!_journal.Visible)
				_journal.Open();				
			else
				_journal.Close();
		}
	}

	public Player GetPlayer()
	{
		return _player;
	}

	public void SetPlayer(Player player)
	{
		_player = player;
		_journal = _player.GetNode<Camera2D>("Camera2D").GetNode<Journal>("Journal");
		_journal.Visible = false;
		_player.EnemyAggroed += OnAggro;
		_player.Position = _playerSpawn;
		GetNode<Elevator>("Elevator").SetLevelSelect(_player.GetNode<Control>("Camera2D/BackToPurgatory"));
	}

	protected virtual int GetBackgroundID()
	{
		return 1;
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

		GD.Print("Hell Reactivate");
		
		if (_player != null && _player.CurrentHealth <= 0)
		{
			GD.Print("Player exists at: ", _player.Position);
			_enemyInCombat?.Enable();
			_player.GlobalPosition = _playerSpawn;
			_player.SetHealth(_player.MaxHealth);

			_player.Die();
		}
		else if (_player != null)
		{
			GD.Print("Player exists at: ", _player.Position);
			_enemyInCombat?.Die();
			_enemyInCombat = null;
		}
		else
		{
			GD.Print("Player doesn't exist");
		}
	}

	public Node UseElevator(string dest = "")
	{
		GD.Print(dest);
		MasterScene.GetInstance().SetPlayerHP(GetPlayer().CurrentHealth);
		// MasterScene.GetInstance().CallDeferred("ActivatePreviousScene", true);
		MasterScene.GetInstance().TotalCoins = _player.Coins;		
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
