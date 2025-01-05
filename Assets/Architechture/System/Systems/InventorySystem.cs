using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using System.Linq;

public class InventorySystem : ASystem, IOnUpdate
{
    private GameObject _UGUIPanel;
#region test
    //private Item_GUI _item_GUI_Prefab;
    //private List<Scriptable_ItemData> _item_Data_List;
#endregion

    private AInventory _toInventory;
    private AInventory _fromInventory;

    private float _oldAngle;
    private float _newAngle;
    private float _rotateAngle;
    private GUI_Item _draggingObject;
    private ItemData.ItemDir _oldDirection;
    private ItemData.ItemDir _newDirection;

    private Vector3[] _offsetArray = new Vector3[4];
    private Vector3 _positionOffset;
    //private Vector3 _oldPosition;

    //------------------
    private CellNumber _oldCellNum;

    public override void OnSetUp()
    {
        _UGUIPanel = gameStat.inventoryPanel;
        //_item_GUI_Prefab = gameStat.item_GUI;

        gameStat.onInventoryActiveEvent += SwitchInventoryActive;
        
        gameStat.inventoryList[0].itemInstantiateEvent += InstantiateGUI;
        gameStat.inventoryList[1].itemInstantiateEvent += InstantiateGUI;
        gameStat.inventoryList[2].itemInstantiateEvent += InstantiateGUI;
        gameStat.inventoryList[3].itemInstantiateEvent += InstantiateGUI;

        gameStat.inventoryList[2].onInsertEvent += EquipmentInsert;
        gameStat.inventoryList[2].onRemoveEvent += EquipmentRemove;

        gameStat.inventoryList[3].onInsertEvent += EquipmentInsert;
        gameStat.inventoryList[3].onRemoveEvent += EquipmentRemove;

        InventoryPanelActive(gameStat.isInventoryPanelActive);
    }

    public void SwitchInventoryActive()
    {
        gameStat.isInventoryPanelActive = !gameStat.isInventoryPanelActive;

        InventoryPanelActive(gameStat.isInventoryPanelActive);
    }

    public void InventoryPanelActive(bool isActive)
    {
         //InventoryのPanelをオン/オフ
        gameStat.inventoryPanel.SetActive(isActive);

        //Storageの中身をロード/アンロード
        if(isActive) 
        {
            gameStat.inventoryList[0].OpenInventory(gameStat.playerStorage);
            gameStat.inventoryList[1].OpenInventory(gameStat.otherStorage);

            gameStat.inventoryList[2].OpenInventory(gameStat.playerStorage);
            gameStat.inventoryList[3].OpenInventory(gameStat.playerStorage);
        }
        else 
        {
            foreach(AInventory inventory in gameStat.inventoryList)
            {
                inventory.CloseInventory();
            }
        }
    }

    public void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(gameStat.playerStorage.WeaponArray);
            Debug.Log(string.Join(", ", gameStat.playerStorage.WeaponArray.Select(w => w.ObjectData.ItemName)));
        }

        if(_draggingObject != null)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                if(!_draggingObject.Data.ObjectData.CanRotate)return;

                _newDirection = _draggingObject.GetNextDir(_newDirection);//OK
                _oldAngle = _draggingObject.GetRotationAngle(_oldDirection);
                _newAngle = _draggingObject.GetRotationAngle(_newDirection);//OK

                //Debug.Log("newAngle: " + _newAngle);
                _rotateAngle = _newAngle - _oldAngle;//OK
                //Debug.Log("回転: " + _rotateAngle);

                Vector3 offsetVec = _offsetArray[(int)_newDirection];
                //Debug.Log(_newDirection);

                _positionOffset = PositionOffset(offsetVec, Vector3.zero, _rotateAngle);

                _draggingObject.SetRotation(_newDirection);
                _draggingObject.SetPivot(_newDirection);
            }

            _draggingObject.SetPosition(Input.mousePosition + _positionOffset);
            // Debug.Log(_draggingObject.transform.position);
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
        Debug.Log(item.Data.ObjectData.ItemName + "を使った");
    }

    public void EquipmentInsert(int index, ItemData data)
    {
        gameStat.onEquipEvent?.Invoke(index, data);
    }

    public void EquipmentRemove(int index)
    {
        gameStat.onUnEquipEvent?.Invoke(index);
    }

#region 新しい処理
    public GUI_Item InstantiateGUI(ItemData data, Transform transform)
    {
        GUI_Item gui = GameObject.Instantiate(gameStat.gui_Item_Prefab, transform);
        gui.OnSetUp(data);

        gui.onBeginDragEvent += StartDragging;
        gui.onPointerDownEvent += PointerDown;
        gui.onEndDragEvent += EndDragging;

        gui.onUseEvent += ItemUse;

        return gui;
    }

    public void PointerDown(GUI_Item gui)
    {
        // UnityEngine.Debug.Log("クリックした！！");
    }

    public void StartDragging(GUI_Item gui)
    {
        if(!gameStat.isInventoryPanelActive)return;
        if(gui == null)return;

        ItemData itemData = gui.Data;

        _draggingObject = gui;
        _oldDirection = _newDirection = itemData.Direction;
        _fromInventory =  gui.BelongingInventory;
        //_oldPosition = gui.RectTransform.position;
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
            //Debug.Log((i + 1) % 4);
            _offsetArray[i] = corners[(i + 1) % 4] - mousePos;

            //Debug.Log($"{corners[(i + 1) % 4]} : {_offsetArray[i].magnitude}");
            //Debug.Log($"{corners[i]} : {corners[i].magnitude}");
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
        foreach (AInventory inventory in gameStat.inventoryList)
        {
            newCell = inventory.ScreenPosToCellNum(newPosition);

            Debug.Log(inventory.IsValid(newCell));

            if (inventory.IsValid(newCell))
            {
                _toInventory = inventory;
                break;
            }
        }

        //Debug.Log(newCell);
        //Debug.Log(_toInventory);

        if (_toInventory != null || gui == null)
        {
            if(_toInventory.CanPlaceItem(gui, newCell, _newDirection))
            {
                //Debug.Log("おけてはいる");
                uint remain = _toInventory.InsertItem(gui, newCell, _newDirection);

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
        _toInventory = null;
    }
#endregion
}
