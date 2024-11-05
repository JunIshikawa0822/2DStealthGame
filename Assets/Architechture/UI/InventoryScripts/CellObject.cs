using UnityEngine;

public class CellObject
{   
    public int position_x;
    public int position_y;

    //このセルオブジェクトがOriginCellの場合、入っているオブジェクトを示す
    private Item_GUI _item;
    //セルに入っているオブジェクトのOriginCellNumを示す
    //現在Stackされている数
    private uint _stackNum;
    private CellNumber _originCellNum;

    private bool _canStackOnCell;
    //private bool isStackableOnCell;

    public CellObject(/*Grid<CellObject> grid,*/ int x, int y) 
    {
        //this.grid = grid;
        position_x = x;
        position_y = y;

        //_stackNum = 0;
        //SetStackability();
        //cellObjectPosition = worldPosition;
    }

    public Item_GUI GetItemInCell(){return _item;}
    public CellNumber GetOriginCellNum(){return _originCellNum;}
    public uint GetStackNum()
    {
        Debug.Log($"このセルに入っている個数 : {_stackNum}");
        return _stackNum;
    }

    public bool CanStack(Scriptable_ItemData item)
    {
        bool canStack = false;

        if(this._item.GetItemData() == item && this._stackNum < item.stackableNum)
        {
            canStack = true;
        }

        return canStack;
    }

    // public void SetStackability()
    // {
    //     if(this.placedObject == null)
    //     {
    //         canStackOnCell = true;
    //         return;
    //     }

    //     int canStackNum = this.placedObject.GetItemData().stackableNum;

    //     if(canStackNum > 0)
    //     {
    //         if(this.stackNum < canStackNum)
    //         {
    //             canStackOnCell = true;
    //             return;
    //         }
    //     }

    //     canStackOnCell = false;
    // }

    // public bool GetStackability()
    // {
    //     return canStackOnCell;
    // }

    public void InsertItem(Item_GUI item)
    {
        _item = item;
        _stackNum++;
        Debug.Log($"StackNum : {_stackNum}");
    }

    public void InsertOriginCellNumber(CellNumber cellNum)
    {
        _originCellNum = cellNum;
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
