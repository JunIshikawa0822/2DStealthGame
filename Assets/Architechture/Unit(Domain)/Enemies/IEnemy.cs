
using UnityEngine;

public interface IEnemy
{
    public void OnSetUp(Entity_HealthPoint enemyHP, Transform gun);
    public void Move();
    public void Rotate();
    public void Attack();
    public void Reload(IGun gun, Entity_Magazine magazine);
    public void Hide();
    //public abstract void SearchAround();
}
