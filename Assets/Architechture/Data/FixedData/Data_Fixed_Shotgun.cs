using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Data_Fixed_Shotgun", menuName = "ScriptableObject/ItemData/Fixed/UnConsumable/Fixed_Shotgun", order = 0)]

public class Data_Fixed_Shotgun : A_Data_Fixed_Gun
{
    [SerializeField] private int _simulNum;
    [SerializeField] private float _spreadAngle;

    public int SimulNum{get => _simulNum;}
    public float SpreadAngle{get => _spreadAngle;}
    
    public override bool IsClickUse {get => false;}
    public override float UseTime{get => 0;}

    public override bool Equals(I_Data_Item data)
    {
        Data_Fixed_Shotgun shotgunData = data as Data_Fixed_Shotgun;
        if(shotgunData == null) return false;

        if(ReferenceEquals(this, data)) return true;

        if(_simulNum != shotgunData.SimulNum) return false;
        if(_spreadAngle != shotgunData.SpreadAngle) return false;

        return base.Equals(data);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(base.GetHashCode());

            hashCode.Add(_simulNum);
            hashCode.Add(_spreadAngle);

            return hashCode.ToHashCode();
        }
    }
}
