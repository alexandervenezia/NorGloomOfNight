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
    

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
        _grabber = GetNode<Sprite2D>("Sprite2D");
        GD.Print("GrabbyPos: ", _grabber.Position);
        Visible = false;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.Owner is Player)
        {
            _abortAttack = false;
            _player = (Player)area.Owner;
            GD.Print("Grabby time");
            GD.Print(_player.Velocity.X);
            _grabber.GlobalPosition = new Vector2(_player.GlobalPosition.X + _player.Velocity.X * _warningTimeMS / 1000f, _grabber.GlobalPosition.Y);
            GD.Print("GrabbyPos: ", _grabber.GlobalPosition);
            StartAttackProcess();
        }
    }

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
            return;
        }

        if (_grabber.GlobalPosition.DistanceTo(_player.GlobalPosition) < _attackRange)
        {
            _player.TakeDamage(_damage);
            GD.Print("Get grabbed!");
        }

        GD.Print("Grabby attack range: ", _grabber.GlobalPosition.DistanceTo(_player.GlobalPosition));

        Visible = false;
        _grabber.Position = Vector2.Zero;
        GD.Print("GrabbyPos: ", _grabber.Position);

        await Task.Delay(1500);
        
        if (_abortAttack)
        {
            Visible = false;
            return;
        }        

        GD.Print("Grabby time again");
        GD.Print(_player.Velocity.X);
        _grabber.GlobalPosition = new Vector2(_player.GlobalPosition.X + _player.Velocity.X * _warningTimeMS / 1000f, _grabber.GlobalPosition.Y);
        GD.Print("GrabbyPos: ", _grabber.GlobalPosition);
        StartAttackProcess();
        StartAttackProcess();
    }
}
