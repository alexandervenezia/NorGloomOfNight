namespace Overworld;

using Godot;
using System;
using System.Diagnostics;
using System.Linq;

[Tool]
public partial class GroundEnemy : Enemy
{
    [Export] Vector2 _relativePatrolBounds;

    private Marker2D _leftPatrolMarker;
    private Marker2D _rightPatrolMarker;
    private int direction;

    public override void _Ready()
    {
        direction = -1;
        base._Ready();
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
            return;

        Velocity = new Vector2(_speed * direction, 0);
        base._Process(delta);
        
        UpdateMarkers();
        
    }

    private void UpdateMarkers()
    {
        Debug.Assert(_relativePatrolBounds.X <= _relativePatrolBounds.Y);
        if (Engine.IsEditorHint())
        {
            if (_leftPatrolMarker == null)
            {
               Node[] nodes = GetChildren().ToArray<Node>();
               
               foreach (Node n in nodes)
               {
                if (n.Name == "PatrolMarkers")
                {
                    Node[] subnodes = n.GetChildren().ToArray<Node>();
                    _leftPatrolMarker = (Marker2D)subnodes[0];
                    _rightPatrolMarker = (Marker2D)subnodes[1];
                    break;
                }
               }
            }
        }
        else
            return;

        _leftPatrolMarker.Position = new Vector2(_relativePatrolBounds.X, _leftPatrolMarker.Position.Y);
        _rightPatrolMarker.Position = new Vector2(_relativePatrolBounds.Y, _rightPatrolMarker.Position.Y);
    }
}
