using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    public void OnSetUp(Entity_HealthPoint playerHP, DrawFieldOfView drawFieldOfView, FindOpponent find, DrawOpponent draw, float viewRadius, float viewAngle);
    public void Move(Vector2 inputDirection);
    public void Attack(AGun gun);
    public void Rotate(Vector3 mouseWorldPosition);
    public void Equip(AGun gun);
    public void DrawView();
    public void Reload(AGun gun, Entity_Magazine magazine);

    //public abstract void SetEquipment(IGun item, int gunIndex);
}
