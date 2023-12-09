namespace Overworld;

using Godot;
using Overworld;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class HellLevel : GameLevel, ILevel
{
	public override void _Ready()
	{
		GD.Print("Hello, Hell.");
	}

	public override void SetPlayer(Player player)
	{
		base.SetPlayer(player);
		GetNode<Elevator>("Elevator").SetLevelSelect(_player.GetNode<Control>("Camera2D/BackToPurgatory"));
	}
}
