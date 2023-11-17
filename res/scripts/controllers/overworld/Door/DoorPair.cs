using Godot;
using System;
using System.Linq;
using System.Numerics;

[Tool]
public partial class DoorPair : Node2D
{
    [Export] private Vector2I _doorOnePos;
    [Export] private Vector2I _doorTwoPos;

    private Sprite2D _doorOne;
    private Sprite2D _doorTwo;

    public override void _Ready()
    {
        Node[] nodes = GetChildren().ToArray<Node>();
        _doorOne = (Door)nodes[0];
        _doorTwo = (Door)nodes[1];
        _doorOne.Position = _doorOnePos;
        _doorTwo.Position = _doorTwoPos;
        ((Door)(_doorOne)).SetPair((Door)_doorTwo);
        ((Door)(_doorTwo)).SetPair((Door)_doorOne);

        _doorOne.Visible = false;
        _doorOne.Visible = false;
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            if (_doorOne == null || _doorTwo == null)
            {
                Node[] nodes = GetChildren().ToArray<Node>();
                _doorOne = (Sprite2D)nodes[0];
                _doorTwo = (Sprite2D)nodes[1];
            }
            else
            {
                _doorOne.Position = _doorOnePos;
                _doorTwo.Position = _doorTwoPos;
            }
            
        }
        else
        {
            
        }
    }
}
