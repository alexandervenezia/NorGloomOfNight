namespace Overworld;

using Godot;
using System;

public interface IQuest
{
    public string GetName();
    public string GetFlavorText();
    public string GetNextStep();
    public bool IsFinished();
    public void CompleteQuest(Player player);
}
