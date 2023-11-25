using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
            // Frames.Add(id, GD.Load<SpriteFrames>(Lookup[id]));
            ResourceLoader.LoadThreadedRequest(Lookup[id]);
        }
    }

    public SpriteFrames GetAsset(int id)
    {
        if (!Frames.ContainsKey(id))
        {
            Frames.Add(id, (SpriteFrames)ResourceLoader.LoadThreadedGet(Lookup[id]));
        }
        return Frames[id];
        //return Frames[id];
    }

    public static EnemyAssetLookup GetInstance()
    {
        return _instance;
    }
}
