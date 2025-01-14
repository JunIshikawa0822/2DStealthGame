using Cysharp.Threading.Tasks;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class CellObject
{   
    public int position_x;
    public int position_y;

    public CellNumber Origin{get; set;}
    private A_Item_GUI _item_GUI;
    public A_Item_GUI GuiInCell{get => _item_GUI;}
    //private uint _count;

    public CellObject(int x, int y) 
    {
        //this.grid = grid;
        position_x = x;
        position_y = y;
    }
    //cellがnull
    //cellにAが入る　Aを入れられる
    //cellにAが入る　Bを入れられる
    //cellにAが入る　もう入らない

    //使う
    public uint Stack(A_Item_GUI insertGUI)
    {
        uint overflow = 0;
        uint add = _item_GUI.Item.StackingNum + insertGUI.Item.StackingNum;

        if(add > _item_GUI.Item.Data.StackableNum)
        {
            overflow = add - _item_GUI.Item.Data.StackableNum;

            _item_GUI.Item.StackingNum = _item_GUI.Item.Data.StackableNum;
            insertGUI.Item.StackingNum = overflow;

            insertGUI.SetStackText(insertGUI.Item.StackingNum);
        }
        else
        {
            _item_GUI.Item.StackingNum += insertGUI.Item.StackingNum;

            insertGUI.OnDestroy();
        }

        _item_GUI.SetStackText(_item_GUI.Item.StackingNum);

        return overflow;
    }

    //使う
    public void Insert(A_Item_GUI insertGUI)
    {
        _item_GUI = insertGUI;

        _item_GUI.SetStackText(_item_GUI.Item.StackingNum);
    }

    //使う
    public bool IsStackable()
    {
        if(_item_GUI == null)return true;
        return _item_GUI.Item.StackingNum < _item_GUI.Item.Data.StackableNum;
    }

    //使う
    public void Reset()
    {
        _item_GUI = null;
        Origin = null;
    }

    //使う
    public bool CheckEquality(IInventoryItem inventoryItem)
    {
        if(_item_GUI == null)return true;

        return _item_GUI.Item.Data.Equals(inventoryItem.Data);
    }
}
