using Godot;
using System;

public partial class FlyingEnemy : Overworld.Enemy
{
    [Export] float _aggroRadius;
    [Export] float _deAggroRange;
    [Export] float _patrolRadius;

    private RayCast2D _raycast;
	private bool _aggro;
	private Overworld.Player _player;

    public override void _Ready()
	{
        _aggro = false;
		_raycast = GetNode<RayCast2D>("RayCast2D");
		base._Ready();
	}

    private Vector2 destTest;
    public override void _PhysicsProcess(double delta)
    {        
        if (_player == null)
            _player = GetOwner<ILevel>().GetPlayer();

        if (!_aggro)
        {    

            Vector2 angleToPosition = Position - _spawnPoint;
            if (angleToPosition.Length() < 10)
            {
                angleToPosition = Vector2.Left * 10f;
            }
 

            Vector2 rotated = angleToPosition.Rotated(0.1f);
            rotated = rotated.Normalized() * _patrolRadius + _spawnPoint;
            Vector2 direction = rotated - Position;

            Vector2 accel = direction.Normalized() * _acceleration * 2f;
            Velocity += accel * (float)delta;

            if (_spawnPoint.DistanceTo(_player.Position) < _aggroRadius)
                _aggro = true;
        }
        else
        {
            Vector2 direction = _player.Position - Position;
            direction = direction.Normalized();

            if (Mathf.Abs(Position.X - _player.Position.X) > 250f)
                direction = (direction + Vector2.Up*0.5f).Normalized();

            Vector2 accel = direction.Normalized() * _acceleration * 10f;
            Velocity += accel * (float)delta;

            if (_spawnPoint.DistanceTo(_player.Position) > _deAggroRange)
                _aggro = false;
        }

        if (Velocity.Length() > _speed)
            Velocity = Velocity.Normalized() * _speed;

        if (_warningLabel != null)
			_warningLabel.Visible = _aggro;

        base._PhysicsProcess(delta);
        Orient();

    }

    private void Orient()
	{

		_sprite.FlipH = Velocity.X < 0;
	}

}
