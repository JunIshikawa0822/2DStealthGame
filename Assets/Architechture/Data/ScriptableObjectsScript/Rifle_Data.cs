using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Rifle_Data", menuName = "ScriptableObject/ItemData/UnConsumable/Gun/Rifle", order = 0)]
public class Rifle_Data : A_Item_Data
{
    [Header("銃の情報")]
    public float muzzleVelocity;
    public float shotInterval;
    public override Type DataType {get => this.GetType();}
}
