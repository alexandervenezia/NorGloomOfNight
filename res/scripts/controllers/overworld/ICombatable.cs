using System.Collections.Generic;
using Godot;

public interface ICombatable
{
    public List<int> GetEnemyIDs();
    public void Disable();
    public void Enable();
    public bool IsEnabled();
    public void Die();
    public bool HasIntroCutscene();
    public Cutscene GetIntroCutscene();
}
