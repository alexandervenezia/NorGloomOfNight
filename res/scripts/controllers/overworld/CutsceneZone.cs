using Godot;
using Overworld;
using System;

public partial class CutsceneZone : Area2D
{
	[Export] public Cutscene Cutscene;
	// This is terrible.
	[Export] public bool FLAG_TALKED_TO_MANAGER;
	[Export] public bool FLAG_ACQUIRED_CROWN;
	[Export] public bool FLAG_BEAT_DOG;
	[Export] public bool FLAG_DELIVERED_CROWN;

	public bool Burned;

	public bool ShouldRun()
	{
		bool talkedToManager = !FLAG_TALKED_TO_MANAGER || QuestManager.GetInstance().FLAG_TALKED_TO_MANAGER;
		bool acquiredCrown = !FLAG_ACQUIRED_CROWN || QuestManager.GetInstance().FLAG_ACQUIRED_CROWN;
		bool beatDog = !FLAG_BEAT_DOG || QuestManager.GetInstance().FLAG_BEAT_DOG;
		bool deliveredCrown = !FLAG_DELIVERED_CROWN || QuestManager.GetInstance().FLAG_DELIVERED_CROWN;

		return talkedToManager && acquiredCrown && beatDog && deliveredCrown && !Burned;
	}
}
