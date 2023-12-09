namespace Overworld;

using Godot;
using Overworld;
using System;
using System.Security.AccessControl;
using System.Threading.Tasks;


public partial class Purgatory : GameLevel, ILevel
{
	[Export] private string _managementUID;
	[Export] private string _prideUID;
	

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
		//_playerSpawn = _player.GlobalPosition;
		//_playerSpawn += Vector2.Right * 1675; // Hardcoded elevator offset; dumb
	}

	protected async override void OnAggro(ICombatable enemy)
	{
		base.OnAggro(enemy);
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

}
