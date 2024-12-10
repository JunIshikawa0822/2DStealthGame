using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] A_Item_Data[] testDataArray;
    List<ItemData> _storageList;
    public List<ItemData> ItemList{get => _storageList;}
    public bool test;

    void Awake()
    {
        _storageList = new List<ItemData>();
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

    public void TestData()
    {
        AddItem(ItemMake(testDataArray[0], 1, new CellNumber(0, 0)));
        AddItem(ItemMake(testDataArray[1], 1, new CellNumber(0, 2)));
        AddItem(ItemMake(testDataArray[2], 1, new CellNumber(0, 4)));
        AddItem(ItemMake(testDataArray[3], 1, new CellNumber(0, 6)));
        AddItem(ItemMake(testDataArray[4], 2, new CellNumber(0, 8)));
    }

    public ItemData ItemMake(A_Item_Data data, uint num, CellNumber place)
    {
        ItemData item = new ItemData(data, num);
        item.Address = place;
        item.Direction = ItemData.ItemDir.Down;
        return item;
    }
}
