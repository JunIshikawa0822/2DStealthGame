using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
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
        _storageList.Remove(data);
    }
}
