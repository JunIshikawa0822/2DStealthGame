using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] A_Item_Data[] testDataArray;
    [SerializeField] A_Item_Data[] testWeaponDataArray = new A_Item_Data[2];
    List<ItemData> _storageList = new List<ItemData>();
    ItemData[] _weaponArray = new ItemData[2];
    // ItemData _gunData1;
    // ItemData _gunData2;
    public List<ItemData> ItemList{get => _storageList;}
    public ItemData[] WeaponArray{get => _weaponArray;}
    // public ItemData Gun1{get => _gunData1;}
    
    public A_Item_Data[] TestWeaponData{get => testWeaponDataArray; set => testWeaponDataArray = value;}

    public bool test;

    void Awake()
    {
    }

    void Start()
    {
        if(test)
        {
            TestData();
        }
    }

    public void AddItem(ItemData data)
    {
        _storageList.Add(data);
    }
    public void TakeItem(ItemData data)
    {
        _storageList.Remove(data);
    }

    public void AddWeapon(ItemData data, int index)
    {
        _weaponArray[index] = data;
    }

    public void RemoveWeapon(int index)
    {
        _weaponArray[index] = null;
    }

    public void TestData()
    {
        if(testDataArray[0] != null)AddItem(ItemMake(testDataArray[0], 1, new CellNumber(0, 0)));
        if(testDataArray[0] != null)AddItem(ItemMake(testDataArray[1], 1, new CellNumber(0, 2)));
        if(testDataArray[0] != null)AddItem(ItemMake(testDataArray[2], 1, new CellNumber(0, 4)));
        if(testDataArray[0] != null)AddItem(ItemMake(testDataArray[3], 1, new CellNumber(0, 6)));
        if(testDataArray[0] != null)AddItem(ItemMake(testDataArray[4], 2, new CellNumber(0, 8)));

        if(testWeaponDataArray[0] != null)AddWeapon(ItemMake(testWeaponDataArray[0], 1, new CellNumber(0, 0)), 0);
    }

    public ItemData ItemMake(A_Item_Data data, uint num, CellNumber place)
    {
        ItemData item = new ItemData(data, num);
        item.Address = place;
        item.Direction = ItemData.ItemDir.Down;
        return item;
    }
}
