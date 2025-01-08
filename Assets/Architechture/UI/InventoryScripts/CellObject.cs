using Unity.Entities.UniversalDelegates;
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

    private bool _isStackableOnCell;

    public CellNumber Origin{get; set;}
    //public Item_GUI ItemInCell{get => _item;}

    private I_Data_Item _data;
    private uint _count;

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

    public uint InsertItem(I_Data_Item data, uint insertNumber)
    {
        _data = data;

        uint remain = 0;
        if(_count + insertNumber >= _data.StackableNum) 
        {
            remain = _count + insertNumber - _data.StackableNum;
            _count = _data.StackableNum;
        }
        else
        {
            _count += insertNumber;
        }

        return remain;
    }

    public void Reset()
    {
        _data = null;
        Origin = null;
        _count = 0;
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

    public bool CheckEquality(I_Data_Item data)
    {
        if(_data == null)return true;
        if(_data.Equals(data))return true;

        return false;
    }
}
