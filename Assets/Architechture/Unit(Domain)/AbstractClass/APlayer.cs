using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APlayer : AEntity
{
    public abstract void OnMove(Vector2 inputDirection, Vector3 mouseWorldPosition);

    public abstract void OnAttack();

    public abstract void OnReload();

    public abstract void SetEquipment(IGun item, int gunIndex);
}
