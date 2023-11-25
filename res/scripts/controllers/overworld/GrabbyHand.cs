namespace Overworld;

using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

public partial class GrabbyHand : Area2D
{
    [Export] private int _warningTimeMS = 1000;
    [Export] private int _attackRange = 150;
    [Export] private int _damage = 5;
    private bool _abortAttack;
    private Player _player;
    private Sprite2D _grabber;
    private CollisionShape2D _collider;
    
    private Vector2 _grabberSpawn;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
        _grabber = GetNode<Sprite2D>("Sprite2D");
        _grabberSpawn = _grabber.GlobalPosition;
        _collider = GetNode<CollisionShape2D>("CollisionShape2D");
        GD.Print("GrabbyPos: ", _grabber.GlobalPosition);
        Visible = false;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.Owner is Player)
        {
            _player = (Player)area.Owner;
            GD.Print("Grabby time");
            GD.Print(_player.Velocity.X);
            _grabber.GlobalPosition = new Vector2(_player.GlobalPosition.X + _player.Velocity.X * _warningTimeMS / 1000f, _grabber.GlobalPosition.Y);

            GD.Print(_grabberSpawn.X);
            GD.Print(_collider.Shape.GetRect().Abs().Size[0]);
            GD.Print(_grabberSpawn.X - _collider.Shape.GetRect().Abs().Size[0]/4f);

            if (_grabber.GlobalPosition.X < _grabberSpawn.X - _collider.Shape.GetRect().Abs().Size[0]/4f)
                _grabber.GlobalPosition = new Vector2(_grabberSpawn.X-_collider.Shape.GetRect().Abs().Size[0]/4f, _grabber.GlobalPosition.Y);
            if (_grabber.GlobalPosition.X > _grabberSpawn.X + _collider.Shape.GetRect().Abs().Size[0]/4f)
                _grabber.GlobalPosition = new Vector2(_grabberSpawn.X + _collider.Shape.GetRect().Abs().Size[0]/4f, _grabber.GlobalPosition.Y);
                
            GD.Print("GrabbyPos: ", _grabber.GlobalPosition);
            StartAttackProcess();
        }
    }

    /*
    public override void _Process(double delta)
    {
        Visible = true;
        _grabber.GlobalPosition = _grabber.GlobalPosition = new Vector2(_grabberSpawn.X+_collider.Shape.GetRect().Abs().Size[0]/4f, _grabber.GlobalPosition.Y);
    }*/

    private void OnAreaExited(Area2D area)
    {
        _abortAttack = true;
    }

    private async void StartAttackProcess()
    {
        Visible = true;
        await Task.Delay(_warningTimeMS);

        if (_abortAttack)
        {
            Visible = false;
            _grabber.GlobalPosition = _grabberSpawn;
            _abortAttack = false;
            return;
        }

        if (_grabber.GlobalPosition.DistanceTo(_player.GlobalPosition) < _attackRange)
        {
            _player.TakeDamage(_damage);
            GD.Print("Get grabbed!");
        }

        GD.Print("Grabby attack range: ", _grabber.GlobalPosition.DistanceTo(_player.GlobalPosition));

        Visible = false;
        _grabber.GlobalPosition = _grabberSpawn;
        GD.Print("GrabbyPos: ", _grabber.GlobalPosition);

        await Task.Delay(1500);
        
        if (_abortAttack)
        {
            Visible = false;
            _grabber.GlobalPosition = _grabberSpawn;
            _abortAttack = false;
            return;
        }        

        GD.Print("Grabby time again");
        GD.Print(_player.Velocity.X);
        _grabber.GlobalPosition = new Vector2(_player.GlobalPosition.X + _player.Velocity.X * _warningTimeMS / 1000f, _grabber.GlobalPosition.Y);

        if (_grabber.GlobalPosition.X < _grabberSpawn.X - _collider.Shape.GetRect().Abs().Size[0]/4f)
            _grabber.GlobalPosition = new Vector2(_grabberSpawn.X - _collider.Shape.GetRect().Abs().Size[0]/4f, _grabber.GlobalPosition.Y);
        if (_grabber.GlobalPosition.X > _grabberSpawn.X + _collider.Shape.GetRect().Abs().Size[0]/4f)
            _grabber.GlobalPosition = new Vector2(_grabberSpawn.X + _collider.Shape.GetRect().Abs().Size[0]/4f, _grabber.GlobalPosition.Y);

        GD.Print("GrabbyPos: ", _grabber.GlobalPosition);
        StartAttackProcess();
    }
}
