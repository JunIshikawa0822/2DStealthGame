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

    public uint InsertItem(Item_GUI item, uint insertNumber)
    {
        if(_isStackableOnCell == false || item == null)
        {
            return insertNumber;
        }

        //何も入っていない
        if(_item == null)
        {
            _item = item;
            _itemData = item.GetItemData();
        }
        else if(_item != item)
        {
            _item.OnDestroy();
            _item = item;
            _itemData = item.GetItemData();
        }

        // Debug.Log(insertNumber);

        uint remain = insertNumber;
        for(; remain > 0; remain--)
        {
            //stackされている数を上回ったらstackできなくする
            if(_isStackableOnCell == false)
            {
                break;
            }
            _stackNum++;

            if(_stackNum >= _itemData.stackableNum)
            {
                _isStackableOnCell = false;
            }
        }
        
        Debug.Log($"{position_x},{position_y} : {_isStackableOnCell}");

        return remain;
    }

    public void ResetCell()
    {
        _item = null;
        this.Origin = null;
        _itemData = null;
        _isStackableOnCell = true;
        _stackNum = 0;
    }

    public void SetStack()
    {
        if(_item == null)return;
        _item.SetStackNum(_stackNum);
    }

    public bool CheckEquality(Item_GUI item)
    {
        if(_item == null && _itemData == null)
        {
            Debug.Log("そもそもnullなのでEqualityとかない");
            return true;
        }
        
        if(_itemData.itemID == item.GetItemData().itemID)return true;
        else return false;
    }
}
