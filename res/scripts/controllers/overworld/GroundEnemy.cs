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
	[Export] float _aggroSpeedBuff = 1.5f;
	[Export] float FOV;

	const float MAX_LOS = 15000;

	private Marker2D _leftPatrolMarker;
	private Marker2D _rightPatrolMarker;
	private int _direction;

	private RayCast2D _raycast;
	private bool _aggro;
	private Player _player;
	

	public override void _Ready()
	{
		_direction = -1;
		_raycast = GetNode<RayCast2D>("RayCast2D");
		base._Ready();
	}

	private float AggroSpeedBuff()
	{
		if (_aggro)
			return _aggroSpeedBuff;
		return 1f;
	}	

	public override void _PhysicsProcess(double delta)
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
		if (Velocity.Length() > _speed*AggroSpeedBuff())
			Velocity = Velocity.Normalized() * _speed*AggroSpeedBuff();

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

		
		if (_raycast.GetCollider() is Player && Mathf.Abs(Mathf.RadToDeg(cast.AngleTo(Velocity))) < FOV)
		{
			_aggro = true;
			//GD.Print("HIT!");
		}
		else
			_aggro = false;


		if (_player.Position.X < _spawnPoint.X + _relativePatrolBounds.X || _player.Position.X > _spawnPoint.X + _relativePatrolBounds.Y)
		{
			_aggro = false;
		}
		
		if (_aggro)
		{
			_direction = _player.GlobalPosition.X > GlobalPosition.X ? 1 : -1;
			
		}

		if (_warningLabel != null)
			_warningLabel.Visible = _aggro;

		base._PhysicsProcess(delta);        

		Orient();    

			
		
	}

	private void Orient()
	{
		float buffer = 500f;
		if (Position.X < _spawnPoint.X + _relativePatrolBounds.X+buffer)
		{
			_direction = 1;
		}
		else if (Position.X > _spawnPoint.X + _relativePatrolBounds.Y-buffer)
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
