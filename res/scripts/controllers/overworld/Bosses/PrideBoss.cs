namespace Overworld;

using Godot;
using Overworld;

public partial class PrideBoss : Enemy
{
	public override void Die()
	{
		GD.Print("Pride Died");
		QuestManager.GetInstance().FLAG_DELIVERED_CROWN = true;
		QueueFree();
	}

}
