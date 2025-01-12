using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStorage : MonoBehaviour, IStorage
{
    IInventoryItem[] _weaponArray = new IInventoryItem[1];

    public void Add(IInventoryItem item)
    {
        if(_weaponArray[0] != null)
        {
            Debug.Log("追加不可");
            return;
        }

        _weaponArray[0] = item;
    }

    public void Remove(IInventoryItem item)
    {
        int num = Array.IndexOf(_weaponArray, item);

        if(num > 0)
        {
            _weaponArray[0] = null;
        }
        else
        {
            Debug.Log("該当のデータが存在しません");
        }
    }

    public IInventoryItem[] GetItems()
    {
        return _weaponArray;
    }
}
