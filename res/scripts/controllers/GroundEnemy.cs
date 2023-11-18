namespace Overworld;

using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;

[Tool]
public partial class GroundEnemy : Enemy
{
    [Export] Vector2 _relativePatrolBounds;

    private Marker2D _leftPatrolMarker;
    private Marker2D _rightPatrolMarker;
    private int _direction;

    public override void _Ready()
    {
        _direction = -1;
        base._Ready();
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            UpdateMarkers();
            return;
        }


        // Velocity = new Vector2(_speed * direction, 0);
        float accel = _acceleration;

        accel /= (float)System.Math.Log10(Mathf.Abs(Velocity.X) + 2f);
        accel *= Mathf.Log(_speed);
        
        if (Velocity.X > 0 != _direction > 0)
            accel *= 5f;
        



        Velocity += _direction * Vector2.Right * accel * (float)delta;
        if (Velocity.Length() > _speed)
            Velocity = Velocity.Normalized() * _speed;

        base._Process(delta);

        Orient();
        
        
        
    }

    private void Orient()
    {
        if (Position.X < _spawnPoint.X + _relativePatrolBounds.X)
        {
            _direction = 1;
        }
        else if (Position.X > _spawnPoint.X + _relativePatrolBounds.Y)
            _direction = -1;

        //Scale = new Vector2(Mathf.Abs(Scale.X)*direction, Scale.Y);
        //Scale = new Vector2(-1, 1);
        _sprite.FlipH = Velocity.X < 0;
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
