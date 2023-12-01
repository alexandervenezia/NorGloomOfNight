using Godot;
using System;
using System.Threading.Tasks;

public partial class Door : Sprite2D
{
    private Door _pair;
    private ILevel _level;

    public void SetPair(Door pair)
    {
        _pair = pair;

        foreach (Node n in GetTree().Root.GetChild(0, false).GetChildren())
        {
            if (n is ILevel)
            {
                _level = (ILevel)n;
                break;
            }
        }
    }
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_interact") && InRange())
        {
            Enter();
            GD.Print("Entering");
        }

        _level.GetPlayer().SetInteractable(InRange());
    }

    private bool InRange()
    {
        return GlobalPosition.DistanceSquaredTo(_level.GetPlayer().GlobalPosition) < 50000;
    }

    private async void Enter()
    {
        await Task.Delay(150); // Play sound here
        _level.GetPlayer().GlobalPosition = _pair.GlobalPosition + Vector2.Down*15;
    }
}
