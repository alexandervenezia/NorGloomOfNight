using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public partial class MasterAudio : Node
{
    [Export] private float _volume;
    [Export] private float _fadeTimeSeconds;
    [Export] private string _musicPath;
    private AudioStreamPlayer _musicPlayer;
    private static MasterAudio _instance;

    private Queue<string> _queue;
    private Dictionary<string, AudioStream> _preloaded;

    private bool _noRestart;

    public override void _Ready()
    {
        _musicPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        _musicPlayer.VolumeDb = _volume;
        _instance = this;
        _queue = new();

        DirAccess musicDir = DirAccess.Open(_musicPath);
        GD.Print(musicDir.GetCurrentDir());

        string fileName = musicDir.GetNext();
        GD.Print(fileName);

        string[] songs = DirAccess.GetFilesAt(_musicPath);

        _preloaded = new();

        foreach (string song in songs)
        {
            if (!song.EndsWith(".import"))
                continue;
            string path = _musicPath + song.Split(".im")[0];
            string id = ResourceUid.IdToText(ResourceLoader.GetResourceUid(path));
            AudioStream stream = GD.Load<AudioStream>(path);
            
            _preloaded.Add(id, stream);
        }
    }

    public async void PlaySong(string musicUID)
    {
        GD.Print("Play order received for UID: ", musicUID);
        
        if (_musicPlayer.Playing)
        {
            GD.Print("Fading");
            Tween fadeOut = GetTree().CreateTween();
            fadeOut.TweenProperty(_musicPlayer, "volume_db", -50f, _fadeTimeSeconds);

            bool doneTweening = false;

            fadeOut.Finished += () => {
                doneTweening = true;
            };

            while (!doneTweening)
            {
                // await Task.Delay(1000 * (int)_fadeTimeSeconds);
                await Task.Delay(50);
            }
        }
        _musicPlayer.VolumeDb = _volume;
        //_musicPlayer.Stream = GD.Load<AudioStream>(musicUID);
        _musicPlayer.Stream = _preloaded[musicUID];
        GD.Print("Now playing - ", _musicPlayer.Stream.ResourcePath);
        _musicPlayer.Play(fromPosition:0f);
        GD.Print(_musicPlayer.VolumeDb);
    }

    public void ClearQueue()
    {
        _queue.Clear();
    }

    public void QueueSong(string musicUID)
    {
        GD.Print("Queue order received for UID: ", musicUID);
        _queue.Enqueue(musicUID);
    }

    public void MusicFinished()
    {
        GD.Print("Music finished");
        string result;
        if (_queue.TryDequeue(out result))
        {
            GD.Print("Found song in queue");
            PlaySong(result);
        }
    }

    public static MasterAudio GetInstance()
    {
        return _instance;
    }

    public void SetNoRestart()
    {
        _noRestart = true;
    }

    public bool GetNoRestart()
    {
        bool temp = _noRestart;
        _noRestart = false;
        return temp;
    }
}
