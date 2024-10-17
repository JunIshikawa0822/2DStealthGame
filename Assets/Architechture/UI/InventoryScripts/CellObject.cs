public class CellObject
{   
    public int position_x;
    public int position_y;

    //セルに入っているオブジェクトを示す
    private Item_GUI _item;

    private bool _canStackOnCell;

    //現在Stackされている数
    //private int _stackNum;

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

    public Item_GUI GetItemInCell()
    {
        return _item;
    }

    // public int GetStackNum()
    // {
    //     return _stackNum;
    // }

    public bool CanInsertToCellObject(Item_GUI item)
    {
        bool canInsert = false;

        if(_item == null)
        {
            canInsert = true;
        }
        // else if(_item.GetItemData() == item.GetItemData())
        // {
        //     if(_stackNum < item.GetItemData().stackableNum)
        //     {
        //         canInsert = true;
        //     }  
        // }   
        return canInsert;
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

    public void InsertToCellObject(Item_GUI item)
    {
        _item = item;
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
