using UnityEngine;

[CreateAssetMenu(fileName = "Fixed_Data_Handgun", menuName = "ScriptableObject/ItemData/Fixed/UnConsumable/Fixed_Handgun", order = 0)]
public class Fixed_Handgun_Data : A_Data_Fixed_Gun
{
    public override bool IsClickUse {get => false;}
    public override float UseTime{get => 0;}

    public override bool Equals(I_Data_Item data)
    {
        Fixed_Handgun_Data gunData = data as Fixed_Handgun_Data;
        if(gunData == null)return false;

        if(ReferenceEquals(this, data))return true;

        return base.Equals(data);
    }
}
