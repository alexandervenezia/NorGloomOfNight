using Godot;

public partial class Cutscene : Resource
{
    [Export] public string SpeakerName;
    [Export] public string[] Lines;

    // This is terrible.
    [Export] public bool FLAG_TALKED_TO_MANAGER;
    [Export] public bool FLAG_ACQUIRED_CROWN;
    [Export] public bool FLAG_BEAT_DOG;
    [Export] public bool FLAG_DELIVERED_CROWN;
}