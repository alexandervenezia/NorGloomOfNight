using Godot;

namespace Overworld;
public class DeliverCrown : IQuest
{
    private string _name;
    private string _command;
    private string _flavorText;

    public DeliverCrown()
    {
        _name = "Appointed Rounds";
        _flavorText = "You have heard few good things about what is [color=red]Below.[/color] Perhaps your predecessor's leave was not so voluntary as Management would have you believe.";
        _command = "Descend into hell and deliver the [color=gold]Crown[/color] to [color=purple]Pride[/color]. Let no adversity bar the fulfillment of your sacred duty.";
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
        return QuestManager.GetInstance().FLAG_DELIVERED_CROWN;
    }
}