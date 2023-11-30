namespace Overworld;
public class EmptyQuest : IQuest
{
    public void CompleteQuest(Player player)
    {
        throw new System.NotImplementedException();
    }

    public string GetFlavorText()
    {
        return "";
    }

    public string GetName()
    {
        return "Triumph";
    }

    public string GetNextStep()
    {
        return "Your task is complete. Yet your work is never done.";
    }

    public bool IsFinished()
    {
        return false;
    }
}