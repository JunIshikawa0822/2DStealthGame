using System;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Fixed_Food", menuName = "ScriptableObject/ItemData/Fixed/Consumable/Fixed_Food", order = 0)]
public class Data_Fixed_Food : A_Data_Fixed
{
    [Header("食べ物の情報")]
    [SerializeField]private float _hpHealPoint;
    [SerializeField]private float _sanityHealPoint;
    [SerializeField]private float _useTime;

    #region A_Data_Fixed
    public override bool IsClickUse {get => true;}
    public override float UseTime{get => _useTime;}
    #endregion

    public float HPHealPoint{get => _hpHealPoint;}
    public float SanityHealPoint{get => _sanityHealPoint;}

    public override bool Equals(I_Data_Item data)
    {
        Data_Fixed_Food foodData = data as Data_Fixed_Food;
        if(foodData == null)return false;

        if (ReferenceEquals(this, data)) return true;

        if(foodData.HPHealPoint != _hpHealPoint) return false;
        if(foodData.SanityHealPoint != _sanityHealPoint)return false;
        if(foodData.UseTime != _useTime)return false;

        return true;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(base.GetHashCode());

            hashCode.Add(_hpHealPoint);
            hashCode.Add(_sanityHealPoint);
            hashCode.Add(_useTime);

            return hashCode.ToHashCode();
        }
    }
}
