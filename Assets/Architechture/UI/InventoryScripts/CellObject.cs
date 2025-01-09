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

    private A_Item_GUI _item_GUI;
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

    public void InsertItem(GUI_Item gui, uint insertNumber)
    {
        _gui = gui;
        _itemData = _gui.Data;
        _stackNumber = insertNumber;
    }

    public uint Insert(A_Item_GUI insertGUI)
    {
        uint remain = 0;
        //insert
        if(_item_GUI == null) _item_GUI = insertGUI;
        //stack
        else
        {
            //overflow
            if(_item_GUI.Item.StackingNum + insertGUI.Item.StackingNum > _item_GUI.Item.Data.StackableNum)
            {
                remain = _item_GUI.Item.StackingNum + insertGUI.Item.StackingNum - _item_GUI.Item.Data.StackableNum;
                
                _item_GUI.Item.StackingNum = _item_GUI.Item.Data.StackableNum;
                insertGUI.Item.StackingNum = remain; 
            }
            //un overflow
            else
            {
                _item_GUI.Item.StackingNum += insertGUI.Item.StackingNum;
                insertGUI.Item.StackingNum = 0;
            }

            _item_GUI.SetStackText(_item_GUI.Item.StackingNum);
            insertGUI.SetStackText(insertGUI.Item.StackingNum);

            Debug.Log("入れたGUI " + insertGUI.Item.StackingNum);
            Debug.Log("もともと入ってたGUI" + _item_GUI.Item.StackingNum);

            if(insertGUI.Item.StackingNum <= 0)
            {
                insertGUI.OnDestroy();
            }
        }

        return remain;
    }

    public void Reset()
    {
        _item_GUI = null;
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

    
}
