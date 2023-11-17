using Godot;
using Overworld;
using System;

public partial class Purgatory : Node2D, ILevel
{
    private Player _player;
    public Player Player => _player;
    public override void _Ready()
    {
        _player = (Player)GetNode("OverworldPlayer");
        _player.EnemyAggroed += OnAggro;
    }

    private void OnAggro(ICombatable enemy)
    {
        // Either begin combat immediately or after cutscene
        // TODO: Begin combat here
        MasterScene master = MasterScene.GetInstance();
        master.SetEnemyIDs(enemy.GetEnemyIDs());
        master.SetPlayerHP(15);
        master.ActivateScene(master.CombatSceneUID, true, true);
        // master.SetPlayerHP(_player.)
        GD.Print("Aggroed");
    }

    public Player GetPlayer()
    {
        return _player;
    }
}
