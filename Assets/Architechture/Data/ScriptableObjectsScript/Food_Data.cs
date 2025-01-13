using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Food_Data", menuName = "ScriptableObject/ItemData/Consumable/Food", order = 0)]
public class Food_Data : A_Item_Data
{
    [Header("食べ物の情報")]
    public readonly float hpHealPoint;
    public readonly float sanityHealPoint;
    public readonly float useTime;

    public override Type DataType {get => this.GetType();}
}
