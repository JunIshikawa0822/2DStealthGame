using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public abstract class AEnemy : AEntity
{
    public abstract void OnMove();
    public abstract void OnAttack();
    public abstract void OnReload();
}
