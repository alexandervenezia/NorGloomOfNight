using Godot;
using System;

public partial class MainMenu : Node2D
{
	[Export] private string _music;

	public override void _Ready()
	{
		MasterAudio.GetInstance().PlaySong(_music);
	}
}
