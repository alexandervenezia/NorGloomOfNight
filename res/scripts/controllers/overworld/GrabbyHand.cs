namespace Overworld;

using Godot;
using System;
using System.Threading.Tasks;

public partial class GrabbyHand : Area2D
{
    [Export] private int _warningTimeMS = 500;
    [Export] private int _attackRange = 1000;
    [Export] private int _damage = 5;
    private bool _abortAttack;
    private Player _player;
    private Sprite2D _grabber;
    

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
        _grabber = GetNode<Sprite2D>("Sprite2D");
        Visible = false;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.Owner is Player)
        {
            _player = (Player)area.Owner;
            GD.Print("Grabby time");
            _grabber.Position = new Vector2(_player.Position.X + _player.Velocity.X * 100, _grabber.Position.Y);
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

        if (Position.DistanceTo(_player.Position) < _attackRange)
        {
            _player.TakeDamage(_damage);
            GD.Print("Get grabbed!");
        }

        Visible = false;
    }
}
