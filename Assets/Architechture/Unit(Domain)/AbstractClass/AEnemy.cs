using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public abstract class AEnemy : AEntity
{
    public abstract void OnSetUp(Entity_HealthPoint enemyHP);
    public abstract void OnMove();
    public abstract void OnAttack();
    public abstract void OnReload();
    public abstract void OnHide();
    //public abstract void SearchAround();
}
