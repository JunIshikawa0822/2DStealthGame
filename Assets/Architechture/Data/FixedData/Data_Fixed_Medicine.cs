
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Data_Fixed_Medicine", menuName = "ScriptableObject/ItemData/Fixed/Consumable/Fixed_Medicine", order = 0)]
public class Data_Fixed_Medicine : A_Data_Fixed
{
    [Header("薬の情報")]
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
        Data_Fixed_Medicine medicineData = data as Data_Fixed_Medicine;
        if(medicineData == null)return false;

        if (ReferenceEquals(this, data)) return true;

        if(_hpHealPoint != medicineData.HPHealPoint) return false;
        if(_sanityHealPoint != medicineData.SanityHealPoint)return false;
        if(_useTime != medicineData.UseTime)return false;

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
