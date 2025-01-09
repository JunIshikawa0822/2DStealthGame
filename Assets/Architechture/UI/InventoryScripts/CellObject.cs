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

    private IInventoryItem _inventoryItem;
    //private uint _count;

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

    public uint InsertItem(IInventoryItem inventoryItem)
    {
        uint remain = 0;

        if(_inventoryItem == null)
        {
            _inventoryItem = inventoryItem;
        }
        else
        {
            if(_inventoryItem.StackingNum + inventoryItem.StackingNum >= _inventoryItem.Data.StackableNum)
            {
                remain = _inventoryItem.StackingNum + inventoryItem.StackingNum - _inventoryItem.Data.StackableNum;
                _inventoryItem.StackingNum = _inventoryItem.Data.StackableNum;
            }
            else
            {
                _inventoryItem.StackingNum += inventoryItem.StackingNum;
            }
        }

        return remain;
    }

    public void Reset()
    {
        _inventoryItem = null;
        Origin = null;
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

    public bool CheckEquality(IInventoryItem inventoryItem)
    {
        if(inventoryItem.Data == null)return true;
        if(inventoryItem.Data.Equals(_inventoryItem.Data))return true;

        return false;
    }
}
