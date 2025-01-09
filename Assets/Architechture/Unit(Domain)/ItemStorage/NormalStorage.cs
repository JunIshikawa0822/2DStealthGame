using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class NormalStorage : MonoBehaviour, IStorage
{
    List<IInventoryItem> _itemList = new List<IInventoryItem>();

    public void Add(IInventoryItem item)
    {
        _itemList.Add(item);
    }

    public void Remove(IInventoryItem item)
    {
        _itemList.Remove(item);
    }

    public IInventoryItem[] GetItems()
    {
        return _itemList.ToArray();
    }
}
