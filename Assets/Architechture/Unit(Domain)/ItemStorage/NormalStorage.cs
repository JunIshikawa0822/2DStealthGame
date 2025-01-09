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
        int num = _itemList.IndexOf(item);

        if(num > 0)
        {
            _itemList.RemoveAt(num);
        }
        else
        {
            Debug.Log("該当のデータが存在しません");
        }
    }

    public IInventoryItem[] GetItems()
    {
        return _itemList.ToArray();
    }
}
