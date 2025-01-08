
using UnityEngine;
[CreateAssetMenu(fileName = "Data_Fixed_Rifle", menuName = "ScriptableObject/ItemData/Fixed/UnConsumable/Fixed_Rifle", order = 0)]

public class Data_Fixed_Rifle : A_Data_Fixed_Gun
{
    public override bool IsClickUse {get => false;}
    public override float UseTime{get => 0;}
    public override bool Equals(I_Data_Item data)
    {
        Data_Fixed_Rifle rifleData = data as Data_Fixed_Rifle;
        if(rifleData == null)return false;

        if(ReferenceEquals(this, data))return true;

        return base.Equals(data);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
