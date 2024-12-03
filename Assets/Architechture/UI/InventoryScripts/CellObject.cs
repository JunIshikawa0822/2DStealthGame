using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class CellObject
{   
    public int position_x;
    public int position_y;

    

#region 新しいデータ管理に対応したい
    //private GUI_Item _gui_Item;
    private ItemData _itemData;
    private uint _stackNumber;
    private GUI_Item _gui;
    public ItemData DataInCell{get => _itemData;}
    public GUI_Item GUIInCell{get => _gui;}
#endregion

//このセルオブジェクトがOriginCellの場合、入っているオブジェクトを示す
    //private Item_GUI _item;
    //A_Item_Data _itemData;
    //セルに入っているオブジェクトのOriginCellNumを示す
    //現在Stackされている数
    //private uint _stackNum;
    private bool _isStackableOnCell;

    public CellNumber Origin{get; set;}
    //public Item_GUI ItemInCell{get => _item;}

    public CellObject(int x, int y) 
    {
        //this.grid = grid;
        position_x = x;
        position_y = y;

        _isStackableOnCell = true;
        _stackNumber = 0;
    }

    //public Item_GUI GetItemInCell(){return _item;}
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
        if(_itemData == null)return 0;
        uint remain = decreaseNumber;

        Debug.Log(remain + "個減らしたい");
        for(; remain > 0; remain--)
        {
            if(_stackNumber == 0)
            {
                break;
            }
            _stackNumber--;
            //Debug.Log("のこり" + _stackNum + "個:あと" + remain + "へらす");
        }

        if(_stackNumber == 0)
        {
            _gui.OnDestroy();
            ResetCell();
        }

        return _stackNumber;
    }

    public void InsertItem(GUI_Item gui, uint insertNumber)
    {
        _gui = gui;
        _itemData = _gui.Data;
        _stackNumber = insertNumber;
    }

    public uint InsertItem(Item_GUI item, uint insertNumber)
    {
        // if(_isStackableOnCell == false || item == null)
        // {
        //     return insertNumber;
        // }

        // bool itemBreak = true;
        // //何も入っていない
        // if(_gui == null)
        // {
        //     _item = item;
        //     _itemData = item.ItemData;
        //     itemBreak = false;
        // }

        // // Debug.Log(insertNumber);

        uint remain = insertNumber;
        // for(; remain > 0; remain--)
        // {
        //     //stackされている数を上回ったらstackできなくする
        //     if(_isStackableOnCell == false)
        //     {
        //         break;
        //     }
        //     _stackNum++;
        //     item.StackingNum--;

        //     if(_stackNum >= _itemData.StackableNum)
        //     {
        //         _isStackableOnCell = false;
        //     }
        // }
        
        // //Debug.Log($"{position_x},{position_y} : {_isStackableOnCell}");
        // if(itemBreak == true && remain == 0)
        // {
        //     item.OnDestroy();
        // }

        return remain;
    }

    public void ResetCell()
    {
        _gui = null;
        this.Origin = null;
        _itemData = null;
        _isStackableOnCell = true;
        _stackNumber = 0;
    }

    public void SetStack()
    {
        if(_gui == null)return;
        _gui.SetStackNum(_stackNumber);
    }

    // public bool CheckEquality(Item_GUI item)
    // {
    //     if(_item == null && _itemData == null)
    //     {
    //         Debug.Log("そもそもnullなのでEqualityとかない");
    //         return true;
    //     }
        
    //     if(_itemData.ItemID == item.ItemData.ItemID && _itemData.GetType() == item.ItemData.GetType())return true;
    //     else return false;
    // }

    public bool CheckEquality(ItemData data)
    {
        if(_gui == null && _itemData == null)
        {
            Debug.Log("そもそもnullなのでEqualityとかない");
            return true;
        }
        
        if(_itemData.ItemID == data.ItemID)return true;
        else return false;
    }
}
