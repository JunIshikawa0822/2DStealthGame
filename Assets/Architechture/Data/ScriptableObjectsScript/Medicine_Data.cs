using UnityEngine;
using System;

[CreateAssetMenu(fileName = "HandGun_Data", menuName = "ScriptableObject/ItemData/Consumable/Medicine", order = 0)]
public class Medicine_Data : A_Item_Data
{
    [Header("薬の情報")]
    public float hpHealPoint;
    public float sanityHealPoint;
    public float useTime;

    public override Type DataType {get => this.GetType();}
}
