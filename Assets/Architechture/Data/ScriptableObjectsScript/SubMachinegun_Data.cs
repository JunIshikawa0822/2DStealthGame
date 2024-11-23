using UnityEngine;
using System;
[CreateAssetMenu(fileName = "SubMachinegun_Data", menuName = "ScriptableObject/ItemData/UnConsumable/Gun/SubMachinegun", order = 0)]
public class SubMachinegun_Data : A_Item_Data
{
    [Header("銃の情報")]
    public int simulNum;
    public float muzzleVelocity;
    public float shotInterval;
    public override Type DataType {get => this.GetType();}
}
