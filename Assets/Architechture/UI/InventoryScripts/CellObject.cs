using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class CellObject
{   
    public int position_x;
    public int position_y;

    //このセルオブジェクトがOriginCellの場合、入っているオブジェクトを示す
    private Item_GUI _item;
    Scriptable_ItemData _itemData;
    //セルに入っているオブジェクトのOriginCellNumを示す
    //現在Stackされている数
    private uint _stackNum;
    public CellNumber Origin{get; set;}
    private bool _isStackableOnCell;

    public CellObject(int x, int y) 
    {
        //this.grid = grid;
        position_x = x;
        position_y = y;

        _isStackableOnCell = true;
        _stackNum = 0;
    }

    public Item_GUI GetItemInCell(){return _item;}
    //public CellNumber GetOriginCellNum(){return originCellNum;}
    public bool GetStackabilty()
    {
        return _isStackableOnCell;
    }

    //cellがnull
    //cellにAが入る　Aを入れられる
    //cellにAが入る　Bを入れられる
    //cellにAが入る　もう入らない

    public bool CheckItem(Item_GUI item)
    {
        return _item == item;
    }

    public void InsertItem(Item_GUI item)
    {
        if(_isStackableOnCell == false)return;

        //すでに入っているものと異なる(null、上書きの場合が該当)
        if(_item != item)
        {
            _item = item;
            _itemData = item.GetItemData();
        }

        //インクリメント
        _stackNum++;
        item.StackingNum--;

        //stackされている数を上回ったらstackできなくする
        if(_stackNum >= _itemData.stackableNum)
        {
            _isStackableOnCell = false;
        }

        Debug.Log($"StackNum : {_stackNum}");
    }

    public void ResetCell()
    {
        _item = null;
        _itemData = null;
        _isStackableOnCell = true;
        _stackNum = 0;
    }

    public void InsertOriginCellNumber(CellNumber cellNum)
    {
        Origin = cellNum;
    }

    // public void SetStackNum()
    // {
    //     if(_item == null)
    //     {
    //         _stackNum = 0;
    //         return;
    //     }

    //     if(_canStackOnCell == false) return;

    //     #region ん？
    //     _stackNum = _stackNum + 1;
    //     #endregion
    // }
}
