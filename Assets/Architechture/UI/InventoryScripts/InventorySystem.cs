using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField]
    private List<TetrisInventory> _tetrisInventoriesList = new List<TetrisInventory>();

    private TetrisInventory _toInventory;
    private TetrisInventory _fromInventory;

    private float _oldAngle;
    private float _newAngle;
    private float _rotateAngle;
    private Vector3 _oldPositionOffset;
    private Vector3 _newPositionOffset;
    private Item_GUI _draggingObject;
    private CellNumber _oldOriginCellNum;
    private CellNumber _newOriginCellNum;
    private CellNumber _mouseCellNumToOriginCellNumOffset;
    private Item_GUI.ItemDir _oldDirection;
    private Item_GUI.ItemDir _newDirection;



    public void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        if(_draggingObject != null)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                _newDirection = _draggingObject.GetNextDir(_newDirection);//OK
                _newAngle = _draggingObject.GetRotationAngle(_newDirection);//OK
                _rotateAngle = _newAngle - _oldAngle;//OK
                _newPositionOffset = PositionOffset(_oldPositionOffset, Vector3.zero, _rotateAngle);

                _draggingObject.SetRotation(_newDirection);
            }

            _draggingObject.SetPosition(Input.mousePosition + _newPositionOffset);
        }
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
        _oldOriginCellNum = _newOriginCellNum = item.GetBelongingCellNum();
        _oldAngle = _newAngle = item.GetRotationAngle(_oldDirection);

        _fromInventory = item.GetBelongingInventory();

        Vector3 mousePos = Input.mousePosition;
        _oldPositionOffset = _newPositionOffset = /*Canvasを基本とする座標系*/item.transform.position - /*Screenを基本とする座標系*/mousePos;
    }

    public void EndDragging(Item_GUI item)
    {
        Vector3 mousePos = Input.mousePosition;

        //所属Inventoryを探す
        foreach (TetrisInventory inventory in _tetrisInventoriesList)
        {
            
        }

        if (_toInventory != null)
        {
            
        }
        else
        {
            _fromInventory.InsertItemToInventory(item, item.GetBelongingCellNum(), _oldDirection);
        }
    }
}
