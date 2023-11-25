using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyAssetLookup : Node
{
    [Export] public Godot.Collections.Dictionary<int, string> Lookup;
    public Dictionary<int, SpriteFrames> Frames;

    private static EnemyAssetLookup _instance;

    public override void _Ready()
    {
        _instance = this;
        Frames = new();
        foreach (int id in Lookup.Keys)
        {
            Frames.Add(id, GD.Load<SpriteFrames>(Lookup[id]));
        }
    }

    public SpriteFrames GetAsset(int id)
    {
        return Frames[id];
    }

    public static EnemyAssetLookup GetInstance()
    {
        return _instance;
    }
}
