using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class TetrisInventorySystem : ASystem, IOnUpdate
{
    private GameObject _UGUIPanel;

    private A_Inventory _toInventory;
    private A_Inventory _fromInventory;

    private float _oldAngle;
    private float _newAngle;
    private float _rotateAngle;
    private A_Item_GUI _draggingObject;
    private IInventoryItem.ItemDir _oldDirection;
    private IInventoryItem.ItemDir _newDirection;

    private Vector3[] _offsetVecArray = new Vector3[4];
    private Vector3 _positionOffset;
    //------------------
    private CellNumber _oldCellNum;

    public override void OnSetUp()
    {
        _UGUIPanel = gameStat.inventoryPanel;
        //_item_GUI_Prefab = gameStat.item_GUI;

        gameStat.onInventoryActiveEvent += SwitchInventoryActive;
        
        InventoryPanelActive(gameStat.isInventoryPanelActive);

        IFactory item_GUI_Fac = new Item_GUI_CreateConcreteFactory(gameStat.item_GUI_Prefab, StartDragging, EndDragging);
        IObjectPool item_GUI_Objp = new ObjectPool<Item_GUI>(gameStat.item_GUI_PoolTrans, item_GUI_Fac);
        item_GUI_Objp.PoolSetUp(20);

        foreach(A_Inventory inventory in gameStat.inventories)
        {
            inventory.Init(item_GUI_Objp);

            if(inventory is PlayerEquipInventory)
            {
                inventory.InsertAction += EquipmentInsert;
                inventory.RemoveAction += EquipmentRemove;
            }
        }
    }

    public void SwitchInventoryActive()
    {
        gameStat.isInventoryPanelActive = !gameStat.isInventoryPanelActive;
        InventoryPanelActive(gameStat.isInventoryPanelActive);

        //if(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex] == null)return;
//        Debug.Log(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex].Magazine.MagazineRemaining);
    }

    private void InventoryPanelActive(bool isActive)
    {
         //InventoryのPanelをオン/オフ
        gameStat.inventoryPanel.SetActive(isActive);

        //Storageの中身をロード/アンロード
        if(isActive)
        {
            gameStat.inventories[0].OpenInventory(gameStat.playerStorage);
            gameStat.inventories[1].OpenInventory(gameStat.otherStorage);
            gameStat.inventories[2].OpenInventory(gameStat.weaponStorages[0]);
            gameStat.inventories[3].OpenInventory(gameStat.weaponStorages[1]);
        }
        else 
        {
            foreach(A_Inventory inventory in gameStat.inventories)
            {
                inventory.CloseInventory();
            }
        }
    }

    public void OnUpdate()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log(gameStat.playerStorage.WeaponArray);
        //     Debug.Log(string.Join(", ", gameStat.playerStorage.WeaponArray.Select(w => w.ObjectData.ItemName)));
        // }

        if(_draggingObject == null) return;

        if(Input.GetKeyDown(KeyCode.R))
        {
            if(_draggingObject.Item.Data.IsRotate == false)return;

            _newDirection = _draggingObject.GetNextDir(_newDirection);//OK
            _oldAngle = _draggingObject.GetRotationAngle(_oldDirection);
            _newAngle = _draggingObject.GetRotationAngle(_newDirection);//OK

            //Debug.Log("newAngle: " + _newAngle);
            _rotateAngle = _newAngle - _oldAngle;//OK
            //Debug.Log("回転: " + _rotateAngle);

            Vector3 offsetVec = _offsetVecArray[(int)_newDirection];
            //Debug.Log(_newDirection);

            _positionOffset = PositionOffset(offsetVec, Vector3.zero, _rotateAngle);

            _draggingObject.SetRotation(_newDirection);
            _draggingObject.SetPivot(_newDirection);
        }

        _draggingObject.SetPosition(Input.mousePosition + _positionOffset);
        // Debug.Log(_draggingObject.transform.position);
    }

    private Vector3 PositionOffset(Vector3 vec, Vector3 center, float rotation)
    {
        float x = (vec.x - center.x) * Mathf.Cos(rotation * Mathf.Deg2Rad) - (vec.y - center.y) * Mathf.Sin(rotation * Mathf.Deg2Rad);
        float y = (vec.x - center.x) * Mathf.Sin(rotation * Mathf.Deg2Rad) + (vec.y - center.y) * Mathf.Cos(rotation * Mathf.Deg2Rad);

        Vector3 offsetVec = new Vector3(x, y, 0);
        return offsetVec;
    }

    public void ItemUse(Item_GUI gui)
    {
        Debug.Log(gui.Item.Data.ItemName + "を使った");
    }

    public void EquipmentInsert(int index, I_Data_Item data)
    {
        Debug.Log("Systemもいれたといっている");
        gameStat.onPlayerEquipEvent?.Invoke(index, data);
    }

    public void EquipmentRemove(int index, I_Data_Item data)
    {
        Debug.Log("Systemもぬいたといっている");
        gameStat.onPlayerUnEquipEvent?.Invoke(index, data);
    }

    public void PointerDown(A_Item_GUI gui)
    {
        // UnityEngine.Debug.Log("クリックした！！");
    }

    public void StartDragging(A_Item_GUI gui)
    {
        if(!gameStat.isInventoryPanelActive)return;
        if(gui == null)return;

        Vector3 mousePos = gameStat.cursorScreenPosition;

        foreach (A_Inventory inventory in gameStat.inventories)
        {
            bool isCollide = inventory.IsCollide(gui);

            if(isCollide)
            {
                _fromInventory = inventory;
                break;
            }
        }

        IInventoryItem itemData = gui.Item;

        _draggingObject = gui;
        _oldDirection = _newDirection = itemData.Direction;

        _oldCellNum = itemData.Address;

        _rotateAngle = 0;

        Vector3[] corners = new Vector3[4];
        gui.RectTransform.GetWorldCorners(corners);

        //GetWorldCornersはCanvasのRenderModeによって変わるらしい
        //ScreenSpace OverlayならそのままScreen座標
        //ScreenSpace Camera、World Spaceなら、その後WorldToScreenPosで変換する必要がある
        //GetWorldCornersは、左下、左上、右上、右下の順で格納してしまうので、ItemDirの並び順と揃える必要がある

        for(int i = 0; i < corners.Length; i++)
        {
            //Debug.Log((i + 1) % 4);
            _offsetVecArray[i] = corners[(i + 1) % 4] - mousePos;
        }

        // Debug.Log(_oldDirection);
        _positionOffset = _offsetVecArray[(int)_oldDirection];

        _fromInventory.RemoveItem(_oldCellNum);

        gui.SetParent(_UGUIPanel.transform);
        gui.SetPivot(_oldDirection);
    }

    public void EndDragging(A_Item_GUI gui)
    {
        if(!gameStat.isInventoryPanelActive)return;
        
        Vector3 mousePos = gameStat.cursorScreenPosition;;
        Vector3 newPosition = mousePos + _positionOffset;

        //CellNumber newCell = new CellNumber(0, 0);

        //所属Inventoryを探す
        foreach (A_Inventory inventory in gameStat.inventories)
        {
            bool isCollide = inventory.IsCollide(gui);

            if(isCollide)
            {
                _toInventory = inventory;
                break;
            }

            // newCell = inventory.ScreenPosToCellNum(newPosition);

            // if (inventory.IsValid(newCell))
            // {
            //     _toInventory = inventory;
            //     break;
            // }
        }

        if (_toInventory != null || gui == null)
        {
            CellNumber newCell = _toInventory.ScreenPosToCellNum(newPosition);

            if(_toInventory.CanPlaceItem(gui, newCell, _newDirection))
            {
                //Debug.Log("おけてはいる");
                uint overflow = _toInventory.InsertItem(gui, newCell, _newDirection);
                Debug.Log(_toInventory.gameObject.name + "におけたよ！");

                if(overflow > 0)
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
}
