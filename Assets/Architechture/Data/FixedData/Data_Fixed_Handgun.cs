using UnityEngine;

[CreateAssetMenu(fileName = "Data_Fixed_Handgun", menuName = "ScriptableObject/ItemData/Fixed/UnConsumable/Fixed_Handgun", order = 0)]
public class Data_Fixed_Handgun : A_Data_Fixed_Gun
{
    public override bool IsClickUse {get => false;}
    public override float UseTime{get => 0;}

    public override bool Equals(I_Data_Item data)
    {
        Data_Fixed_Handgun handgunData = data as Data_Fixed_Handgun;
        if(handgunData == null)return false;

        if(ReferenceEquals(this, data))return true;

        return base.Equals(data);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
