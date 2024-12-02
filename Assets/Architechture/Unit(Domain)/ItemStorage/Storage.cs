using System.Collections.Generic;

public class Storage
{
    List<ItemData> _storageList;
    public List<ItemData> ItemList{get => _storageList;}

    public Storage()
    {
        _storageList = new List<ItemData>();
    }

    public void AddItem(ItemData data)
    {
        _storageList.Add(data);
    }

    public void TakeItem(ItemData data)
    {
        
    }
}
