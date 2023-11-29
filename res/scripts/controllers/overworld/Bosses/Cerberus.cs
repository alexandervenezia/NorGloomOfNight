namespace Overworld;

using Godot;
using Overworld;

public partial class Cerberus : Enemy
{
	public override void Die()
	{
        GD.Print("Cerberus Died");
		QuestManager.GetInstance().FLAG_BEAT_DOG = true;
		QueueFree();
	}

}
