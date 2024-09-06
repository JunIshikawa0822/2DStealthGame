using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    public void OnMove(Vector2 inputDirection);

    public void OnAttack();

    public void SetEquipment(IGun item, int gunIndex);
}
