using Godot;
using Overworld;
using System;
using System.Security.AccessControl;

public partial class Purgatory : Node2D, ILevel
{
    [Export] private string _prideUID;
    private Player _player;
    public Player Player => _player;
    private Vector2 _playerSpawn;

    private ICombatable _enemyInCombat;

    public override void _Ready()
    {
        _player = (Player)GetNodeOrNull("OverworldPlayer");
        if (_player != null)
            _player.EnemyAggroed += OnAggro;
        _playerSpawn = _player.GlobalPosition;
    }

    private async void OnAggro(ICombatable enemy)
    {
        GD.Print(enemy.IsEnabled());

        ((Node)enemy).CallDeferred("Disable");
        _enemyInCombat = enemy;
        GD.Print("Aggroed");
        // Either begin combat immediately or after cutscene
        // TODO: Begin combat here
        MasterScene master = MasterScene.GetInstance();
        master.SetEnemyIDs(enemy.GetEnemyIDs());
        master.SetPlayerHP(_player.CurrentHealth);
        master.CallDeferred("ActivateScene", master.CombatSceneUID, true, true);
        
    }

    public Player GetPlayer()
    {
        return _player;
    }

    public void SetPlayer(Player player)
    {
        _player = player;
        _player.EnemyAggroed += OnAggro;
    }

    // TESTING TOOL
    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Delete))
        {
            Node pride = MasterScene.GetInstance().ActivateScene(_prideUID, true, true);
            _player.EnemyAggroed -= OnAggro;
            RemoveChild(_player);
            pride.AddChild(_player);
            ((Pride)pride).SetPlayer(_player);
        }
    }

    public void Reactivate()
    {

        _player?.SetHealth(MasterScene.GetInstance().LoadPlayerHP());
        if (_player != null && _player.CurrentHealth <= 0)
        {
            _enemyInCombat?.Enable();
            _player.GlobalPosition = _playerSpawn;
            _player.SetHealth(_player.MaxHealth);
        }
    }

}
