namespace UI;

using Godot;
using System;

public enum VolumeTypes
{
    MASTER,
    EFFECTS,
    UI,
    MUSIC
}

public partial class Settings : Node
{
    [Export] private CheckButton _fullscreen;
    [Export] private HSlider _masterVolume;
    [Export] private HSlider _effectsVolume;
    [Export] private HSlider _uiVolume;
    [Export] private HSlider _musicVolume;

    private static int _musicServer = -1;
    private static int _masterServer = -1;
    private static int _effectsServer = -1;
    private static int _uiServer = -1;

    public override void _Ready()
    {
        _fullscreen.ButtonPressed = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;
        _fullscreen.Toggled += OnFullscreenPressed;

        _masterVolume.ValueChanged += (double x) => {
            VolumeChanged((float)x, VolumeTypes.MASTER);
        };
        _effectsVolume.ValueChanged += (double x) => {
            VolumeChanged((float)x, VolumeTypes.EFFECTS);
        };
        _uiVolume.ValueChanged += (double x) => {
            VolumeChanged((float)x, VolumeTypes.UI);
        };
        _musicVolume.ValueChanged += (double x) => {
            VolumeChanged((float)x, VolumeTypes.MUSIC);
        };
        
        if (_masterServer == -1)
        {
            _masterServer = AudioServer.GetBusIndex("Master");
            _effectsServer = AudioServer.GetBusIndex("SFX");
            _uiServer = AudioServer.GetBusIndex("UI");
            _musicServer = AudioServer.GetBusIndex("Music");

            _masterVolume.Value = 50;
            _effectsVolume.Value = 50;
            _uiVolume.Value = 50;
            _musicVolume.Value = 50;
        }
        else
        {
            _masterVolume.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_masterServer)) * 50f;
            _effectsVolume.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_effectsServer)) * 50f;
            _uiVolume.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_uiServer)) * 50f;
            _musicVolume.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_musicServer)) * 50f;
        }
    }

    private void OnFullscreenPressed(bool toggled)
    {
        if (toggled)
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        else
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
    }

    private void VolumeChanged(float newVol, VolumeTypes type)
    {
        GD.Print(newVol);
        GD.Print("Volume is " + newVol + "; " + Mathf.LinearToDb(newVol * 0.02f));

        switch (type)
        {
            case VolumeTypes.MASTER:
            AudioServer.SetBusVolumeDb(_masterServer, Mathf.LinearToDb(newVol * 0.02f));
            break;
            case VolumeTypes.EFFECTS:
            AudioServer.SetBusVolumeDb(_effectsServer, Mathf.LinearToDb(newVol * 0.02f));
            break;
            case VolumeTypes.UI:
            AudioServer.SetBusVolumeDb(_uiServer, Mathf.LinearToDb(newVol * 0.02f));
            break;
            case VolumeTypes.MUSIC:
            AudioServer.SetBusVolumeDb(_musicServer, Mathf.LinearToDb(newVol * 0.02f));
            break;
        }

    }
}
