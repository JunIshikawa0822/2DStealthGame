using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Handgun_Data", menuName = "ScriptableObject/ItemData/UnConsumable/Gun/Handgun", order = 0)]
public class Handgun_Data : A_Item_Data
{
    [Header("銃の情報")]
    public float muzzleVelocity;
    public float shotInterval;
    public override Type DataType {get => this.GetType();}
}
