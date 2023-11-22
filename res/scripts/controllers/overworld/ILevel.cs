using Godot;
using Overworld;

public interface ILevel
{
    public Player GetPlayer();
    public void SetPlayer(Player player);
    public void Reactivate();
    public Node UseElevator(string dest="");
}