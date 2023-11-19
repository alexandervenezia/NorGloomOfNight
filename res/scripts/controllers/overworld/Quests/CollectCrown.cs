using Godot;

namespace Overworld;
public class CollectCrown : IQuest
{
    private string _name;
    private string _command;
    private string _flavorText;

    public CollectCrown()
    {
        _name = "Coronation";
        _flavorText = "A [color=gold]Crown[/color] for the [color=purple]Queen[/color] [color=red]Below[color]. Odd: you would not have thought those [color=gold]Above[/color] would have spared so kind a thought for her.";
        _command = "Head to the Mailroom and collect the [color=gold]Crown[/color] from <Poker Guy's name>.";
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
        return QuestManager.GetInstance().FLAG_ACQUIRED_CROWN;
    }
}