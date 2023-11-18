using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial class Elevator : Marker2D
{
    ILevel _level;
    [Export] Control _levelSelect;

    public override void _Ready()
    {
        _level = GetOwner<ILevel>();
    }
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_interact") && InRange())
        {
            // _level.UseElevator();

        }

        _levelSelect.Visible = InRange();
    }

    private bool InRange()
    {
        return GlobalPosition.DistanceSquaredTo(_level.GetPlayer().GlobalPosition) < 50000;
    }
}
