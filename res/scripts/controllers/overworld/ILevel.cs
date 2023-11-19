using Overworld;

public interface ILevel
{
    public Player GetPlayer();
    public void SetPlayer(Player player);
    public void Reactivate();
    public void UseElevator(string dest="");
}