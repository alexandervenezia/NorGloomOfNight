/*
 @author Alexander Venezia (Blunderguy)
*/

namespace Combat;

using Data;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Systems.Combat;


public partial class CombatMain : Node
{
    [ExportGroup("Right click .tscn -> get UID -> paste here. Blame Godot for having to use freaking strings because they still haven't fixed a bug reported over a year ago: https:[slashslash]github.com[slash]godotengine[slash]godot[slash]issues[slash]62916")]
    [Export] private Godot.Collections.Array<string> _enemyTypePaths;
    [Export] private PackedScene _cardResource;
    private Dictionary<int, PackedScene> _enemyTypesByUID;

    private Node2D _rewardsNode;
    private bool _isOver = false;
    public bool IsFightOver => _isOver;

    public Vector2[] EnemySpawnPoints =
    {
        new Vector2(325, -100),
        new Vector2(-230, -150),
        new Vector2(-25, 50),
        new Vector2(525, 150)
    };

    private List<Drop> _loot;

    private void PopulateEnemyArray(ref ICombatant[] enemies)
    {

        _enemyTypesByUID = new();

        // This is stupid, but that's Godot's fault.
        for (int i = 0; i < _enemyTypePaths.Count; i++)
        {
            PackedScene converted = GD.Load<PackedScene>(_enemyTypePaths[i]);
            if (converted == null)
                continue;

            Enemy tempInstance = (Enemy)converted.Instantiate();

            if (_enemyTypesByUID.ContainsKey(tempInstance.UID))
                throw new Exception("Two enemies conflict with shared UID " + tempInstance.UID);

            _enemyTypesByUID.Add(tempInstance.UID, converted);
            tempInstance.QueueFree();
        }
        Node enemiesParentNode = GetNode("Enemies");

        int[] enemyUIDs;
        int[] loaded = null;
        loaded = ((MasterScene)GetTree().Root.GetChild(0)).LoadEnemyIDs()?.ToArray();

        if (loaded != null)
            enemyUIDs = loaded;
        else
        {
            GD.Print("No enemy data received from overworld.");
            enemyUIDs = new int[] { 1, 2, 1, 1 };
        }

        enemies = new ICombatant[enemyUIDs.Length];

        int count = 0;
        Enemy newEnemy;
        foreach (int e in enemyUIDs)
        {
            newEnemy = (Enemy)_enemyTypesByUID[e].Instantiate();
            
            //RemoveChild(newEnemy);
            enemiesParentNode.AddChild(newEnemy);
            newEnemy.GlobalPosition = EnemySpawnPoints[count];
            enemies[count] = newEnemy;
            count++;
        }

    }




    /*
    public void OnTreeEntered()
    {
        GD.Print("Entering tree");
        ICombatant[] enemies = null;
        PopulateEnemyArray(ref enemies);
    }
    */

    public override void _Ready()
    {
        _rewardsNode = GetNode<Node2D>("Rewards");

        ICombatant[] enemies = null;
        PopulateEnemyArray(ref enemies);

        _loot = new();

        foreach (Enemy e in enemies)
        {
            _loot.Add(e.GetDrop());
        }

        Player player = (Player)GetNode("Player");
        CombatManager.GetInstance().NewFight(player, enemies);
    }

    public void SetPlayerHP(int hp)
    {
        ((Player)GetNode("Player")).SetHP(hp);
    }

    public int GetPlayerHP()
    {
        return ((Player)GetNode("Player")).CurrentHealth;
    }

    public async void EndFight(EndState result)
    {
        _isOver = true;
        GD.Print("Is over - " + result);

        MasterDeck.PlayerDeck.ForceFullReshuffle();

        bool rewardsFinished = false;
        HandleRewards(ref rewardsFinished);

        await Task.Delay(500); // Provide a pleasant pause after combat ends

        while (!rewardsFinished)
        {
            await Task.Delay(100);
        }
        await Task.Delay(10000);

        // await Task.Delay(2000); // This can be removed but it will cause non-fatal errors from the floating text's async lifetime function trying to tick after the FloatingTextFactory node is destroyed.

        MasterScene.GetInstance().SetPlayerHP(GetPlayerHP());
        MasterScene.GetInstance().CallDeferred("ActivatePreviousScene", true);
    }

    private void HandleRewards(ref bool doneFlag_ref)
    {
        Card testCard = (Card)_cardResource.Instantiate();
        testCard.UpdateData(MasterDeck.CardTypes[0]);
        
        _rewardsNode.AddChild(testCard);

        foreach (Drop d in _loot)
        {
            GD.Print("Gold: " + d.Gold);
            GD.Print("Card: " + d.Card);
        }

        doneFlag_ref = true;
    }

}
