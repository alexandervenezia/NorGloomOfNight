using Godot;
using Overworld;
using System;
using System.Collections.Generic;

public partial class Pride : HellLevel
{
	//[Export] private Godot.Collections.Dictionary<int, Marker2D> _backgroundChangeMarkers;
	[Export] private Godot.Collections.Array<int> _backgroundIDs;
	[Export] private Godot.Collections.Array<Marker2D> _backgroundMarkers;

	// Do you want to know why this variable is necessary? The answer is Godot.
	// For some God-forsaken reason, when I export a Godot.Collections.Dictionary<int, Marker2D> (note that Godot, in its infinite wisdom, chooses not to support native C# dicts for serialization),
	// the Marker2D fields are always null, despite showing a value in the editor.
	// Changing to two "Arrays" magically fixes this. Thanks, Godot.
	private Dictionary<int, Marker2D> _nonStupidChangeMarkers;


	public override void _Ready()
	{
		base._Ready();

		GD.Print("Running Pride Ready()");

		_nonStupidChangeMarkers = new();
		for (int i = 0; i < _backgroundIDs.Count; i++)
		{
			GD.Print("Adding " + _backgroundIDs[i]);
			GD.Print(_backgroundMarkers[i]);
			_nonStupidChangeMarkers.Add(_backgroundIDs[i], _backgroundMarkers[i]);
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (OS.IsDebugBuild() && Input.IsKeyPressed(Key.R))
		{
			QuestManager.GetInstance().FLAG_TALKED_TO_MANAGER = true;
			QuestManager.GetInstance().FLAG_ACQUIRED_CROWN = true;
			// QuestManager.GetInstance().FLAG_BEAT_DOG = true;

			if (Input.IsActionJustPressed("Select"))
			{
				_player.DebugTeleport(_player.GetLocalMousePosition());
			}
		}
	}

	protected override int GetBackgroundID()
	{
		foreach (int bg in _nonStupidChangeMarkers.Keys)
		{
			GD.Print(_player);
			GD.Print(bg);
			//GD.Print(_backgroundChangeMarkers[2]);

			GD.Print(_nonStupidChangeMarkers);
			GD.Print(_nonStupidChangeMarkers[bg]);

			if (_player.GlobalPosition.X >= _nonStupidChangeMarkers[bg].GlobalPosition.X)
				return bg;
		}
		return 1;
	} 

}
