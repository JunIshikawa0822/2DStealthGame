
using UnityEngine;

public interface IEnemy
{
    public void OnSetUp(Entity_HealthPoint enemyHP);
    public void Move();
    public void Rotate();
    public void Attack();
    public void Reload(AGun gun, Entity_Magazine magazine);
    public void Hide();
    public void Equip(AGun gun);
    //public abstract void SearchAround();
}
