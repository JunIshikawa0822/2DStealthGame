using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class CellObject
{   
    public int position_x;
    public int position_y;

    //このセルオブジェクトがOriginCellの場合、入っているオブジェクトを示す
    private Item_GUI _item;
    IObjectData _itemData;
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

    public uint DecreaseItem(uint decreaseNumber)
    {
        if(_item == null)return 0;
        uint remain = decreaseNumber;

        Debug.Log(remain + "個減らしたい");
        for(; remain > 0; remain--)
        {
            if(_stackNum == 0)
            {
                break;
            }
            _stackNum--;
            //Debug.Log("のこり" + _stackNum + "個:あと" + remain + "へらす");
        }

        if(_stackNum == 0)
        {
            _item.OnDestroy();
            ResetCell();
        }

        return _stackNum;
    }

    public uint InsertItem(Item_GUI item, uint insertNumber)
    {
        if(_isStackableOnCell == false || item == null)
        {
            return insertNumber;
        }

        bool itemBreak = true;
        //何も入っていない
        if(_item == null)
        {
            _item = item;
            _itemData = item.GetItemData();
            itemBreak = false;
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
            item.StackingNum--;

            if(_stackNum >= _itemData.StackableNum)
            {
                _isStackableOnCell = false;
            }
        }
        
        //Debug.Log($"{position_x},{position_y} : {_isStackableOnCell}");
        if(itemBreak == true && remain == 0)
        {
            item.OnDestroy();
        }

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
        
        if(_itemData.ItemID == item.GetItemData().ItemID)return true;
        else return false;
    }
}
