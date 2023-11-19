namespace Overworld;
public class EmptyQuest : IQuest
{
    public void CompleteQuest(Player player)
    {
        throw new System.NotImplementedException();
    }

    public string GetFlavorText()
    {
        throw new System.NotImplementedException();
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }

    public string GetNextStep()
    {
        throw new System.NotImplementedException();
    }

    public bool IsFinished()
    {
        return false;
    }
}