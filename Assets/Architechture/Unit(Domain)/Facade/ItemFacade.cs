
using System.Collections.Generic;

public class ItemFacade
{
    Dictionary<int, Food> _foodInstancesDic;
    Dictionary<int, Medicine> _medicineInstancesDic;
    AEntity _player;

    public ItemFacade(AEntity entity, Food_Data[] foodDataArray, Medicine_Data[] medicineDataArray)
    {
        foreach(Food_Data data in foodDataArray)_foodInstancesDic.Add(data.ItemID, new Food(data, _player));
        foreach(Medicine_Data data in medicineDataArray) _medicineInstancesDic.Add(data.ItemID, new Medicine(data, _player));
    }

    public void ItemUse(A_Item_Data data)
    {
        if(data is Food_Data) _foodInstancesDic[data.ItemID].ItemUse();
        if(data is Medicine_Data) _medicineInstancesDic[data.ItemID].ItemUse();
    }
}
