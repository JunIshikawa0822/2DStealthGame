using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.AI;

public class InventorySystem : ASystem
{
    private Canvas _canvas;
#region test
    private Item_GUI _item_GUI_Prefab;
    private List<Scriptable_ItemData> _item_Data_List;
    private List<IInventory> _inventoriesList;
#endregion

    private IInventory _toInventory;
    private IInventory _fromInventory;

    private float _oldAngle;
    private float _newAngle;
    private float _rotateAngle;
    private Item_GUI _draggingObject;
    private Item_GUI.ItemDir _oldDirection;
    private Item_GUI.ItemDir _newDirection;

    private Vector3[] _offsetArray = new Vector3[4];
    private Vector3 _positionOffset;
    private Vector3 _oldPosition;
    // private Vector3 _newPosition;

    public override void OnSetUp()
    {
        _canvas = gameStat.canvas;
        _item_GUI_Prefab = gameStat.item_GUI;
        _item_Data_List = gameStat.item_Data_List;
        _inventoriesList = gameStat.inventoriesList;
        
        Item_GUI instance1 = InstantiateObject(_item_Data_List[0], 5);
        _inventoriesList[0].InsertItemToInventory(instance1, new CellNumber(0,0), /*instance1.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum1*/);
        
        //test1.anchoredPosition = _tetrisInventoriesList[0].grid.GetCellOriginAnchoredPosition(0, 0);
        Item_GUI instance2 = InstantiateObject(_item_Data_List[0], 1);
        _inventoriesList[1].InsertItemToInventory(instance2, new CellNumber(4,5), /*instance2.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum2*/);

        Item_GUI instance3 = InstantiateObject(_item_Data_List[0], 3);
        _inventoriesList[0].InsertItemToInventory(instance3, new CellNumber(0,2), /*instance3.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum3*/);

        Item_GUI instance4 = InstantiateObject(_item_Data_List[0], 2);
        _inventoriesList[0].InsertItemToInventory(instance4, new CellNumber(0,4), /*instance4.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum4*/);

        Item_GUI instance5 = InstantiateObject(_item_Data_List[0], 1);
        _inventoriesList[0].InsertItemToInventory(instance5, new CellNumber(0,6), /*instance5.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum5*/);

        Item_GUI instance6 = InstantiateObject(_item_Data_List[0], 4);
        _inventoriesList[0].InsertItemToInventory(instance6, new CellNumber(0,8), /*instance6.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum6*/);
    }

    public void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        if(_draggingObject != null)
        {
            if(!_draggingObject.GetItemData().canRotate)return;

            if(Input.GetKeyDown(KeyCode.R))
            {
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
                _draggingObject.SetAnchor(_newDirection);
            }

            _draggingObject.SetPosition(Input.mousePosition + _positionOffset);
        }
    }

    private Item_GUI InstantiateObject(Scriptable_ItemData itemData, uint stackNum)
    {
        //Debug.Log(stackNum);
        //Debug.Log(itemData.stackableNum);
        if(stackNum > itemData.stackableNum)
        {
            stackNum = itemData.stackableNum;
        }
        else if(stackNum < 1)
        {
            stackNum = 1;
        }

        Item_GUI item = GameObject.Instantiate(_item_GUI_Prefab, _canvas.transform);
        item.OnSetUp(itemData);
        item.StackingNum = stackNum;

        item.onBeginDragEvent += StartDragging;
        //item.onDragEvent += OnDragging;
        item.onEndDragEvent += EndDragging;

        return item;
    }

    private Vector3 PositionOffset(Vector3 vec, Vector3 center, float rotation)
    {
        float x = (vec.x - center.x) * Mathf.Cos(rotation * Mathf.Deg2Rad) - (vec.y - center.y) * Mathf.Sin(rotation * Mathf.Deg2Rad);
        float y = (vec.x - center.x) * Mathf.Sin(rotation * Mathf.Deg2Rad) + (vec.y - center.y) * Mathf.Cos(rotation * Mathf.Deg2Rad);

        Vector3 offsetVec = new Vector3(x, y, 0);
        return offsetVec;
    }

    public void StartDragging(Item_GUI item)
    {
        if(item == null)return;

        _draggingObject = item;
        _oldDirection = _newDirection = item.GetDirection();
        _fromInventory = item.GetBelongingInventory();
        _oldPosition = item.GetRectTransform().position;

        _rotateAngle = 0;

        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        item.GetRectTransform().GetWorldCorners(corners);

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

        Debug.Log(_oldDirection);
        _positionOffset = _offsetArray[(int)_oldDirection];
        item.transform.SetParent(_canvas.transform);
    }

    public void EndDragging(Item_GUI item)
    {
        Vector3 mousePos = Input.mousePosition;

        _fromInventory.RemoveItemFromInventory(item, _oldPosition, _oldDirection);

        Vector3 newPosition = mousePos + _positionOffset;

        //所属Inventoryを探す
        foreach (IInventory inventory in _inventoriesList)
        {
            //CellNumber cellNum = inventory.ScreenPosToCellNum(newPosition);

            if (inventory.IsValid(newPosition))
            {
                _toInventory = inventory;
                break;
            }
        }

        if (_toInventory != null || item == null)
        {
            if(_toInventory.CanPlaceItem(item, newPosition, _newDirection))
            {
                Debug.Log("おけてはいる");
                uint remain = _toInventory.InsertItemToInventory(item, newPosition, _newDirection);
                Debug.Log("置けた");

                if(remain > 0)
                {
                    //remain分はromInventoryに再度格納
                    _fromInventory.InsertItemToInventory(item, _oldPosition, _oldDirection);
                }
            }
            else
            {
                _fromInventory.InsertItemToInventory(item, _oldPosition, _oldDirection);
                Debug.Log("置けなかった");
            }
        }
        else
        {
            Debug.Log("toInventory、itemがなかった");
            _fromInventory.InsertItemToInventory(item, _oldPosition, _oldDirection);
        }

        _draggingObject = null;
    }
}
