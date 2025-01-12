
public class Medicine : IObject
{
    float _hpHealPoint;
    float _sanityHealPoint;
    float _useTime;
    string _medicineName;
    public string Name{get => _medicineName;}
    public Medicine(I_Data_Item data)
    {
        if(data is Data_Fixed_Medicine medicineData)
        {
            _hpHealPoint = medicineData.HPHealPoint;
            _sanityHealPoint = medicineData.SanityHealPoint;
            _useTime = medicineData.UseTime;
            _medicineName = data.ItemName;
        }
    }

    public void ItemUse()
    {
        UnityEngine.Debug.Log($"{Name}をつかったよ");
    }
}
