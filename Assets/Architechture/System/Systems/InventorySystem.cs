using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class InventorySystem : ASystem, IOnUpdate
{
    private GameObject _UGUIPanel;
#region test
    private Item_GUI _item_GUI_Prefab;
    //private List<Scriptable_ItemData> _item_Data_List;
#endregion

    private Inventory _toInventory;
    private Inventory _fromInventory;

    private float _oldAngle;
    private float _newAngle;
    private float _rotateAngle;
    private GUI_Item _draggingObject;
    private ItemData.ItemDir _oldDirection;
    private ItemData.ItemDir _newDirection;

    private Vector3[] _offsetArray = new Vector3[4];
    private Vector3 _positionOffset;
    private Vector3 _oldPosition;

    //------------------

    private CellNumber _oldCellNum;

    public override void OnSetUp()
    {
        _UGUIPanel = gameStat.inventoryPanel;
        _item_GUI_Prefab = gameStat.item_GUI;

        gameStat.onInventoryActiveEvent += SwitchInventoryActive;
        PanelLoad();

        gameStat.inventory1.itemInstantiateEvent += InstantiateGUI;
    }

    public void SwitchInventoryActive()
    {
        gameStat.isInventoryPanelActive = !gameStat.isInventoryPanelActive;
        PanelLoad();
    }

    public void PanelLoad()
    {
        gameStat.inventoryPanel.SetActive(gameStat.isInventoryPanelActive);
        //Cursor.visible = gameStat.isInventoryPanelActive;
    }
    

    public void OnUpdate()
    {
        // if(!gameStat.isInventoryPanelActive)
        // {
        //     if(_draggingObject != null)
        //     {
        //         _fromInventory.InsertItemToInventory(_draggingObject, _oldPosition, _oldDirection);
        //         _draggingObject = null;
        //     }
        //     return;
        // }

        if(_draggingObject != null)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                if(!_draggingObject.Data.Object.CanRotate)return;

                _newDirection = _draggingObject.GetNextDir(_newDirection);//OK
                _oldAngle = _draggingObject.GetRotationAngle(_oldDirection);
                _newAngle = _draggingObject.GetRotationAngle(_newDirection);//OK

                Debug.Log("newAngle: " + _newAngle);
                _rotateAngle = _newAngle - _oldAngle;//OK
                Debug.Log("回転: " + _rotateAngle);

                Vector3 offsetVec = _offsetArray[(int)_newDirection];
                Debug.Log(_newDirection);

                _positionOffset = PositionOffset(offsetVec, Vector3.zero, _rotateAngle);

                _draggingObject.SetRotation(_newDirection);
                _draggingObject.SetPivot(_newDirection);
            }

            _draggingObject.SetPosition(Input.mousePosition + _positionOffset);
        }
    }

    private Vector3 PositionOffset(Vector3 vec, Vector3 center, float rotation)
    {
        float x = (vec.x - center.x) * Mathf.Cos(rotation * Mathf.Deg2Rad) - (vec.y - center.y) * Mathf.Sin(rotation * Mathf.Deg2Rad);
        float y = (vec.x - center.x) * Mathf.Sin(rotation * Mathf.Deg2Rad) + (vec.y - center.y) * Mathf.Cos(rotation * Mathf.Deg2Rad);

        Vector3 offsetVec = new Vector3(x, y, 0);
        return offsetVec;
    }

    public void ItemUse(GUI_Item item)
    {
        Debug.Log(item.Data.Object.ItemName + "を使った");
    }

#region 新しい処理
    public GUI_Item InstantiateGUI(ItemData data, Transform transform)
    {
        GUI_Item gui = GameObject.Instantiate(gameStat.gui_Item_Prefab, transform);
        gui.OnSetUp(data);

        gui.onBeginDragEvent += StartDragging;
        gui.onPointerDownEvent += PointerDown;
        gui.onEndDragEvent += EndDragging;

        return gui;
    }

    public void PointerDown(GUI_Item gui)
    {
        UnityEngine.Debug.Log("クリックした！！");
    }

    public void StartDragging(GUI_Item gui)
    {
        if(!gameStat.isInventoryPanelActive)return;
        if(gui == null)return;

        ItemData itemData = gui.Data;

        _draggingObject = gui;
        _oldDirection = _newDirection = itemData.Direction;
        _fromInventory = gui.BelongingInventory;
        _oldPosition = gui.RectTransform.position;
        _oldCellNum = itemData.Address;

        _rotateAngle = 0;

        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        gui.RectTransform.GetWorldCorners(corners);

        //GetWorldCornersはCanvasのRenderModeによって変わるらしい
        //ScreenSpace OverlayならそのままScreen座標
        //ScreenSpace Camera、World Spaceなら、その後WorldToScreenPosで変換する必要がある
        //GetWorldCornersは、左下、左上、右上、右下の順で格納してしまうので、ItemDirの並び順と揃える必要がある

        for(int i = 0; i < corners.Length; i++)
        {
            Debug.Log((i + 1) % 4);
            _offsetArray[i] = corners[(i + 1) % 4] - mousePos;

            //Debug.Log($"{corners[(i + 1) % 4]} : {_offsetArray[i].magnitude}");
            Debug.Log($"{corners[i]} : {corners[i].magnitude}");
        }

        // Debug.Log(_oldDirection);
        _positionOffset = _offsetArray[(int)_oldDirection];
        gui.transform.SetParent(_UGUIPanel.transform);
        gui.SetPivot(_oldDirection);
    }

    public void EndDragging(GUI_Item gui)
    {
        if(!gameStat.isInventoryPanelActive)return;
        
        Vector3 mousePos = Input.mousePosition;

        _fromInventory.RemoveItem(_oldCellNum);

        Vector3 newPosition = mousePos + _positionOffset;

        CellNumber newCell = new CellNumber(0, 0);

        //所属Inventoryを探す
        foreach (Inventory inventory in gameStat.inventoryList)
        {
            newCell = inventory.ScreenPosToCellNum(newPosition);

            if (inventory.IsValid(newCell))
            {
                _toInventory = inventory;
                break;
            }
        }

        if (_toInventory != null || gui == null)
        {
            if(_toInventory.CanPlaceItem(gui, newCell, _newDirection))
            {
                Debug.Log("おけてはいる");
                uint remain = _toInventory.InsertItem(gui, newCell, _newDirection);
                Debug.Log("置けた");

                if(remain > 0)
                {
                    //remain分はromInventoryに再度格納
                    _fromInventory.InsertItem(gui, _oldCellNum, _oldDirection);
                }
            }
            else
            {
                _fromInventory.InsertItem(gui, _oldCellNum, _oldDirection);
                Debug.Log("置けなかった");
            }
        }
        else
        {
            Debug.Log("toInventory、itemがなかった");
            _fromInventory.InsertItem(gui, _oldCellNum, _oldDirection);
        }

        _draggingObject = null;
    }
#endregion
}
