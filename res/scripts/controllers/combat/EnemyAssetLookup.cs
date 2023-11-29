using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EnemyAssetLookup : Node
{
    [Export] public Godot.Collections.Dictionary<int, string> Lookup;
    public Dictionary<int, SpriteFrames> Frames;

    [Export] public Godot.Collections.Dictionary<int, string> Backgrounds;
    public Dictionary<int, Texture2D> BackgroundLookup;

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

        BackgroundLookup = new();
        foreach (int id in Backgrounds.Keys)
        {
            GD.Print("BGID: ", id);
            ResourceLoader.LoadThreadedRequest(Backgrounds[id]);
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

    public Texture2D GetCombatBackground(int id)
    {
        if (!BackgroundLookup.ContainsKey(id))
        {
            BackgroundLookup.Add(id, (Texture2D)ResourceLoader.LoadThreadedGet(Backgrounds[id]));
        }
        return BackgroundLookup[id];
    }

    public static EnemyAssetLookup GetInstance()
    {
        return _instance;
    }
}
