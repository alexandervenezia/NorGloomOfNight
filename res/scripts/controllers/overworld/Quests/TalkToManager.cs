using Godot;

namespace Overworld;
public class TalkToManager : IQuest
{
    private string _name;
    private string _command;
    private string _flavorText;

    public TalkToManager()
    {
        _name = "Office Politics";
        _flavorText = "Mike's gone on break. You've passingly familiar with the concept.";
        _command = "Take the elevator up to your supervisor in Middle Management.";
    }
    public void CompleteQuest(Player player)
    {
        GD.Print("You expected a reward? Get back to work.");
    }

    public string GetFlavorText()
    {
        return _flavorText;
    }

    public string GetName()
    {
        return _name;
    }

    public string GetNextStep()
    {
        return _command;
    }

    public bool IsFinished()
    {
        return QuestManager.GetInstance().FLAG_TALKED_TO_MANAGER;
    }
}