using System.Diagnostics;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class ItemData
{
    public ItemData(A_Item_Data item_Data, uint stackNum)
    {
        _ObjectData = item_Data;

        //UnityEngine.Debug.Log(item_Data);
        _itemID = item_Data.ItemID;

        if(stackNum > item_Data.StackableNum)_stackNum = item_Data.StackableNum;
        else _stackNum = stackNum;
    }
    int _itemID;
    A_Item_Data _ObjectData;
    CellNumber _address;
    ItemDir _direction;
    (uint, uint) _size;
    uint _stackNum;

    public CellNumber Address {set => _address = value; get => _address; }
    public ItemDir Direction {set => _direction = value; get => _direction;}
    public uint StackingNum {set => _stackNum = value; get => _stackNum;}
    public (uint, uint) Size {set => _size = value; get => _size;}
    public A_Item_Data ObjectData {get => _ObjectData;}

    public int ItemID{ get => _itemID;}

    public enum ItemDir
    {
        Down,
        Right,
        Up,
        Left,
        Middle
    }
}
