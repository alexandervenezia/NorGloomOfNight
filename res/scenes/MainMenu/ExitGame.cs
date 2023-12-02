using Godot;
using System;

public partial class ExitGame : Button
{
    public override void _Ready()
	{
		Pressed += () => {
			// GetTree().Quit();
			GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
		};
	}
}
