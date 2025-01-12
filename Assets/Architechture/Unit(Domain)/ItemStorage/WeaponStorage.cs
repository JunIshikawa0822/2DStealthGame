using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponStorage : MonoBehaviour, IStorage
{
    IInventoryItem[] _weaponArray = new IInventoryItem[1];

    [SerializeField]private List<ScriptableObject> _testDataList = new List<ScriptableObject>();

    void Awake()
    {
        
    }
    void Start()
    {
        TestDataLoad(_testDataList);
    }

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

    private void TestDataLoad(List<ScriptableObject> testDataList)
    {
        if(testDataList.Count < 1) return;
        I_Data_Item data = testDataList[0] as I_Data_Item;

        _weaponArray[0] = new InventoryItem(data, 1);
    }

    public IInventoryItem[] GetItems()
    {
        return _weaponArray;
    }
}
