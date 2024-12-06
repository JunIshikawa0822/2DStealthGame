using Microsoft.Unity.VisualStudio.Editor;
using Unity.Entities.UniversalDelegates;

public class ItemData
{
    public ItemData(A_Item_Data item_Data, uint stackNum)
    {
        _Object = item_Data;

        if(stackNum > item_Data.StackableNum)_stackNum = item_Data.StackableNum;
        else _stackNum = stackNum;
    }
    int _itemID;
    A_Item_Data _Object;
    CellNumber _address;
    ItemDir _direction;
    (uint, uint) _size;
    uint _stackNum;

    public CellNumber Address {set => _address = value; get => _address; }
    public ItemDir Direction {set => _direction = value; get => _direction;}
    public uint StackingNum {set => _stackNum = value; get => _stackNum;}
    public (uint, uint) Size {set => _size = value; get => _size;}
    public A_Item_Data Object {get => _Object;}

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
