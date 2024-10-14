using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TetrisInventorySystem : ASystem,IOnUpdate
{
    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private List<TetrisInventory> _tetrisInventoriesList = new List<TetrisInventory>();
    private TetrisInventory _toInventory;
    private TetrisInventory _fromInventory;

    private Vector3 _dragPositionOffset;

    //マウスから左下への補正位置
    private Vector2Int _mouseCellNumToOriginCellNumOffset;
    //private Vector2Int mouseCellNumToMaxCellNumOffset;
    private Scriptable_UI_Item _draggingItemData;

    //private Scriptable_UI_Item.ItemDir originDireciton;
    private Scriptable_UI_Item.ItemDir _itemDireciton;
    private PlacedObject _draggingObject;

    private Vector3 _objectVector;

    //private float _originDirectionAngle;
    private float _nextDirectionAngle;

    private float _rotationAngle;

    [SerializeField]
    private List<Scriptable_UI_Item> _item_Data_List;

    [SerializeField]
    private RectTransform test1;

    [SerializeField]
    private RectTransform test2;


    public void Start()
    {
        OnSetUp();
    }

    public override void OnSetUp()
    {
        PlacedObject instance1 = InstantiatePlacedObject(_item_Data_List[0], 5);
        _tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,0), instance1, instance1.GetStackNum(), instance1.GetDirection(), out int remainNum1);
        
        PlacedObject instance2 = InstantiatePlacedObject(_item_Data_List[0], 5);
        _tetrisInventoriesList[1].InsertItemToInventory(new Vector2Int(4,5), instance2, instance2.GetStackNum(), instance2.GetDirection(), out int remainNum2);

        PlacedObject instance3 = InstantiatePlacedObject(_item_Data_List[0], 5);
        _tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,2), instance3, instance3.GetStackNum(), instance3.GetDirection(), out int remainNum3);

        PlacedObject instance4 = InstantiatePlacedObject(_item_Data_List[0], 5);
        _tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,4), instance4, instance4.GetStackNum(), instance4.GetDirection(), out int remainNum4);

        PlacedObject instance5 = InstantiatePlacedObject(_item_Data_List[0], 5);
        _tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,6), instance5, instance5.GetStackNum(), instance5.GetDirection(), out int remainNum5);

        PlacedObject instance6 = InstantiatePlacedObject(_item_Data_List[0], 5);
        _tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,8), instance6, instance6.GetStackNum(), instance6.GetDirection(), out int remainNum6);
    }

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
                _itemDireciton = _draggingItemData.GetNextDir(_itemDireciton);
                _nextDirectionAngle = _draggingItemData.GetRotationAngle(_itemDireciton);
                _rotationAngle = _nextDirectionAngle - _draggingObject.GetItemData().GetRotationAngle(_draggingObject.GetDirection());
                // Debug.Log(rotationAngle);

                _draggingObject.SetRotation(Quaternion.Euler(0, 0, _nextDirectionAngle));

                //掴んだ時(StartDrag)のmouse→anchorPositionのベクトルを、何度回転させるかという処理
                //あくまで掴んだ時点のベクトルを回転させるため、差分の度数を引数として挿入
                _dragPositionOffset = PositionOffset(_rotationAngle, Vector3.zero, _objectVector);

                Debug.Log("補正" + _mouseCellNumToOriginCellNumOffset);
                Vector2Int offset = _draggingItemData.GetCellNumRotateOffset(_draggingObject.GetDirection(), _itemDireciton, _mouseCellNumToOriginCellNumOffset);
                //Debug.Log(offset);
            }

            //位置補正
            _draggingObject.GetRectTransform().position = Input.mousePosition + _dragPositionOffset;
            //Test(draggingObject);
        }
    }

    // private void Test(PlacedObject placedObject)
    // {
    //     Vector3 mousePos = Input.mousePosition;
    //     Vector2Int mousePosNum = ScreenPosToCellNum(mousePos, fromInventory);

    //     Vector2Int offset = draggingItemData.GetCellNumRotateOffset(originDireciton, _itemDireciton, mouseCellNumToOriginCellNumOffset);

    //     Vector2Int originCellNum = mousePosNum - offset;

    //     Debug.Log("mousePos : " + mousePosNum +  " , originPos : " + originCellNum + ", " + _itemDireciton);
    // }

    private Vector3 PositionOffset(float rotation, Vector3 center, Vector3 vec)
    {
        float x = (vec.x - center.x) * Mathf.Cos(rotation * Mathf.Deg2Rad) - (vec.y - center.y) * Mathf.Sin(rotation * Mathf.Deg2Rad);
        float y = (vec.x - center.x) * Mathf.Sin(rotation * Mathf.Deg2Rad) + (vec.y - center.y) * Mathf.Cos(rotation * Mathf.Deg2Rad);

        Vector3 offsetVec = new Vector3(x, y, 0);
        //Debug.Log(offsetVec.magnitude);
        return offsetVec;
    }

    private PlacedObject InstantiatePlacedObject(Scriptable_UI_Item itemData, int stackNum)
    {
        //Debug.Log(itemData);
        if(stackNum > itemData.stackableNum)
        {
            stackNum = itemData.stackableNum;
        }
        else if(stackNum < 1)
        {
            stackNum = 1;
        }

        Transform instance = GameObject.Instantiate(itemData.prefab, _canvas.transform);
        PlacedObject placedObject = instance.GetComponent<PlacedObject>();
        placedObject.OnSetUp(itemData);
        placedObject.StackNumInit(stackNum);

        placedObject.onBeginDragEvent += StartDragging;
        placedObject.onDragEvent += OnDragging;
        placedObject.onEndDragEvent += EndDragging;

        return placedObject;
    }

    public void StartDragging(PlacedObject placedObject)
    {
        placedObject.OnDragStart();
        //ドラッグしているオブジェクトをキャッシュ
        _draggingObject = placedObject;
        //itemの方向をキャッシュ
        _itemDireciton = placedObject.GetDirection();

        #region いる？
        //originDireciton = _itemDireciton;
        #endregion

        //所属inventoryをキャッシュ
        _fromInventory = placedObject.GetBelongingInventory();

        #region いる？
        //itemDataをキャッシュ
        _draggingItemData = placedObject.GetItemData();
        #endregion

        //rotationの補正をリセット
        _rotationAngle = 0;

        #region いる？
        //取得時の回転をキャッシュ
        //_originDirectionAngle = draggingItemData.GetRotationAngle(_itemDireciton);
        #endregion

        Vector3 mousePos = Input.mousePosition;
        Cursor.visible = true;
        
        //補正用位置取得
        _objectVector = /*Canvasを基本とする座標系*/placedObject.GetRectTransform().position - /*Screenを基本とする座標系*/mousePos;
        _dragPositionOffset = _objectVector;

        //Inventory上でマウス座標を補足し、マウス座標に対応するGrid座標に変換
        Vector2Int mouseNum = ScreenPosToCellNum(mousePos, _fromInventory);

        //マウスのGrid座標から現在いるGrid座標を引くことで、マス目補正を取得
        _mouseCellNumToOriginCellNumOffset = mouseNum - placedObject.GetBelongingCellNum();
        //親子関係をcanvasに変更（すべてのオブジェクトよりも前にいくことでBackground問題を解決する）
        placedObject.GetRectTransform().SetParent(_canvas.transform);
    }

    private Vector2Int ScreenPosToCellNum(Vector2 position, TetrisInventory inventory)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.inventoryRectTransform, position, null, out Vector2 convertPosition);
        return inventory.grid.GetCellNum(convertPosition);
    }

    public void OnDragging(PlacedObject placedObject)
    {
        //Cursor.visible = false;
    }

    public void EndDragging(PlacedObject placedObject)
    {
        placedObject.OnDragEnd();

        Vector3 mousePos = Input.mousePosition;
        Cursor.visible = true;

        Vector2Int mouseNum = new Vector2Int(0,0);
        Vector2Int originCellNum = placedObject.GetBelongingCellNum();

        _fromInventory.RemoveItemFromInventory(placedObject.GetBelongingCellNum(), placedObject, placedObject.GetDirection());

        //originCellNum取得のための補正確認
        Vector2Int cellNumOffset = _draggingItemData.GetCellNumRotateOffset(placedObject.GetDirection(), _itemDireciton, _mouseCellNumToOriginCellNumOffset);
        // Debug.Log("回転に合わせた補正 : " + itemDireciton + " , " + cellNumOffset);

        //所属Inventoryを探す
        foreach (TetrisInventory inventory in _tetrisInventoriesList)
        {
            mouseNum = ScreenPosToCellNum(mousePos, inventory);
            originCellNum = mouseNum - cellNumOffset;

            if (inventory.grid.IsValidCellNum(originCellNum))
            {
                _toInventory = inventory;
                break;
            }
        }

        // Debug.Log("入るCell : " + originCellNum);
        int remainNum = 0;

        if (_toInventory != null)
        {
            if(_toInventory.CanPlaceItem(placedObject, originCellNum, _itemDireciton))
            {
                _toInventory.InsertItemToInventory(originCellNum, placedObject, placedObject.GetStackNum(), _itemDireciton, out remainNum);
                // Debug.Log("おけてはいる");
                Debug.Log("置けた");
                _draggingObject = null;
            }
            else
            {
                originCellNum = placedObject.GetBelongingCellNum();
                _fromInventory.InsertItemToInventory(originCellNum, placedObject, placedObject.GetStackNum(), placedObject.GetDirection(), out remainNum);
                _draggingObject = null;
                Debug.Log("置けなかった");
            }
        }
        else
        {
            originCellNum = placedObject.GetBelongingCellNum();
            _fromInventory.InsertItemToInventory(originCellNum, placedObject, placedObject.GetStackNum(), placedObject.GetDirection(), out remainNum);
            _draggingObject = null;
            Debug.Log("toInventoryがなかった");
        }

        //Debug.Log(remainNum);

        if(remainNum > 0)
        {
            PlacedObject newInstance = InstantiatePlacedObject(_draggingItemData, remainNum);
            _fromInventory.InsertItemToInventory(originCellNum, newInstance, newInstance.GetStackNum(), newInstance.GetDirection(), out remainNum);
            Debug.Log("置き直した");
        }
    }
}
