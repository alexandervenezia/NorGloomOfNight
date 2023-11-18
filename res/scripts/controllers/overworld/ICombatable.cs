using System.Collections.Generic;

public interface ICombatable
{
    public List<int> GetEnemyIDs();
    public void Disable();
    public void Enable();
    public bool IsEnabled();
    public void Die();
}
