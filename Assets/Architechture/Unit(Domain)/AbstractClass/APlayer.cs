using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APlayer : AEntity
{
    public abstract void OnSetUp(Entity_HealthPoint playerHP, DrawFieldOfView drawFieldOfView, FindOpponent find, DrawOpponent draw, float viewRadius, float viewAngle);
    public abstract void OnMove(Vector2 inputDirection, Vector3 mouseWorldPosition);

    public abstract void OnAttack(IGun gun);

    public abstract void DrawView();

    public abstract void OnReload(IGun gun, Entity_Magazine magazine);

    //public abstract void SetEquipment(IGun item, int gunIndex);
}
