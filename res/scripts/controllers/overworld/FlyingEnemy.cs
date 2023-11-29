namespace Overworld;

using Godot;
using System;

public partial class FlyingEnemy : Overworld.Enemy
{
	[Export] float _aggroRadius;
	[Export] float _deAggroRange;
	[Export] float _patrolRadius;
	[Export] float _aggroSpeedBuff = 1.5f;

	const float MAX_LOS = 15000;

	private RayCast2D _raycast;
	private bool _aggro;
	private Overworld.Player _player;

	public override void _Ready()
	{
		_aggro = false;
		_raycast = GetNode<RayCast2D>("RayCast2D");
		base._Ready();
	}

	private float AggroSpeedBuff()
	{
		if (_aggro)
			return _aggroSpeedBuff;
		return 1f;
	}	

	private Vector2 destTest;
	public override void _PhysicsProcess(double delta)
	{        
		if (_player == null)
			_player = GetOwner<ILevel>().GetPlayer();

		Vector2 cast = Vector2.Zero;
		if (_player != null)
		{   
			cast = _player.GlobalPosition - GlobalPosition;
			cast *= 2.5f;
			if (cast.Length() > MAX_LOS)
			{
				cast = cast.Normalized() * MAX_LOS;
			}
			_raycast.TargetPosition = cast * 2.5f;
		}  

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

			if (_raycast.GetCollider() is Player && _spawnPoint.DistanceTo(_player.Position) < _aggroRadius)
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

			if (_raycast.GetCollider() is not Player || _spawnPoint.DistanceTo(_player.Position) > _deAggroRange)
				_aggro = false;
		}

		if (Velocity.Length() > _speed * AggroSpeedBuff())
			Velocity = Velocity.Normalized() * _speed * AggroSpeedBuff();

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
