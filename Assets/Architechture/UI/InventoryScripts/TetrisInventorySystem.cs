using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TetrisInventorySystem : MonoBehaviour, IOnUpdate
{
    [SerializeField]
    private Canvas _canvas;
    [SerializeField]
    private List<TetrisInventory> _tetrisInventoriesList = new List<TetrisInventory>();
    [SerializeField]
    private Item_GUI item_GUI_Prefab;

    private TetrisInventory _toInventory;
    private TetrisInventory _fromInventory;

    private Vector3 _dragPositionOffset;

    //マウスから左下への補正位置
    private Vector2Int _mouseCellNumToOriginCellNumOffset;
    //private Vector2Int mouseCellNumToMaxCellNumOffset;
    
    //private Scriptable_UI_Item _draggingItemData;

    //private Scriptable_UI_Item.ItemDir originDireciton;
    private Item_GUI.ItemDir _itemDireciton;
    private Item_GUI _draggingObject;

    private Vector3 _objectVector;

    //private float _originDirectionAngle;
    private float _nextDirectionAngle;

    private float _rotateAngle;

    //テスト用データたち
    [SerializeField]
    private List<Scriptable_ItemData> _item_Data_List;

    [SerializeField]
    private RectTransform test1;

    [SerializeField]
    private RectTransform test2;


    public void Start()
    {
        OnSetUp();
    }

    public void OnSetUp()
    {
        // Item_GUI instance1 = InstantiateObject(_item_Data_List[0], 5);
        // _tetrisInventoriesList[0].InsertItemToInventory(instance1, new Vector2Int(0,0), /*instance1.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum1*/);
        
        // Item_GUI instance2 = InstantiateObject(_item_Data_List[0], 5);
        // _tetrisInventoriesList[1].InsertItemToInventory(instance2, new Vector2Int(4,5), /*instance2.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum2*/);

        // Item_GUI instance3 = InstantiateObject(_item_Data_List[0], 5);
        // _tetrisInventoriesList[0].InsertItemToInventory(instance3, new Vector2Int(0,2), /*instance3.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum3*/);

        // Item_GUI instance4 = InstantiateObject(_item_Data_List[0], 5);
        // _tetrisInventoriesList[0].InsertItemToInventory(instance4, new Vector2Int(0,4), /*instance4.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum4*/);

        // Item_GUI instance5 = InstantiateObject(_item_Data_List[0], 5);
        // _tetrisInventoriesList[0].InsertItemToInventory(instance5, new Vector2Int(0,6), /*instance5.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum5*/);

        // Item_GUI instance6 = InstantiateObject(_item_Data_List[0], 5);
        // _tetrisInventoriesList[0].InsertItemToInventory(instance6, new Vector2Int(0,8), /*instance6.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum6*/);
    }

    public void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_tetrisInventoriesList[0].GetContainer(), Input.mousePosition, null, out Vector2 convertPosition);
        Debug.Log($"セル座標{_tetrisInventoriesList[0].grid.GetCellNum(null, convertPosition)}");
        Debug.Log($"mouse座標{convertPosition}");

        if(_draggingObject != null)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {   
                _itemDireciton = _draggingObject.GetNextDir(_itemDireciton);
                _nextDirectionAngle = _draggingObject.GetRotationAngle(_itemDireciton);
                _rotateAngle = _nextDirectionAngle - _draggingObject.GetRotationAngle(_draggingObject.GetDirection());

                _draggingObject.SetRotation(Quaternion.Euler(0, 0, _nextDirectionAngle));

                //掴んだ時(StartDrag)のmouse→anchorPositionのベクトルを、何度回転させるかという処理
                //あくまで掴んだ時点のベクトルを回転させるため、差分の度数を引数として挿入
                _dragPositionOffset = PositionOffset(_rotateAngle, Vector3.zero, _objectVector);

                //Debug.Log("補正" + _mouseCellNumToOriginCellNumOffset);
                //Vector2Int offset = _draggingObject.GetCellNumRotateOffset(_draggingObject.GetDirection(), _itemDireciton, _mouseCellNumToOriginCellNumOffset);
                //Debug.Log(offset);
            }

            //位置補正
            _draggingObject.SetPosition(Input.mousePosition + _dragPositionOffset);
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

    private Item_GUI InstantiateObject(Scriptable_ItemData itemData, int stackNum)
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

        Item_GUI item = GameObject.Instantiate(item_GUI_Prefab, _canvas.transform);
        item.OnSetUp(itemData);

        #region あとで
        item.StackInit(stackNum);
        #endregion

        item.onBeginDragEvent += StartDragging;
        item.onDragEvent += OnDragging;
        item.onEndDragEvent += EndDragging;

        return item;
    }

    public void StartDragging(Item_GUI item)
    {
        //ドラッグしているオブジェクトをキャッシュ
        _draggingObject = item;
        //itemの方向を取得
        _itemDireciton = item.GetDirection();

        #region いる？
        //originDireciton = _itemDireciton;
        #endregion

        //所属inventoryをキャッシュ
        _fromInventory = item.GetBelongingInventory();

        #region いる？
        //itemDataをキャッシュ
        //_draggingItemData = placedObject.GetItemData();
        #endregion

        //rotationの補正をリセット
        _rotateAngle = 0;

        #region いる？
        //取得時の回転をキャッシュ
        //_originDirectionAngle = draggingItemData.GetRotationAngle(_itemDireciton);
        #endregion

        Vector3 mousePos = Input.mousePosition;
        Cursor.visible = true;
        
        //補正用位置取得
        _objectVector = /*Canvasを基本とする座標系*/item.transform.position - /*Screenを基本とする座標系*/mousePos;
        _dragPositionOffset = _objectVector;

        //Inventory上でマウス座標を補足し、マウス座標に対応するGrid座標に変換
        Vector2Int mouseNum = ScreenPosToCellNum(mousePos, _fromInventory);

        //マウスのGrid座標から現在いるGrid座標を引くことで、マス目補正を取得
        _mouseCellNumToOriginCellNumOffset = mouseNum - item.GetBelongingCellNum();
        //親子関係をcanvasに変更（すべてのオブジェクトよりも前にいくことでBackground問題を解決する）
        item.transform.SetParent(_canvas.transform);
    }

    private Vector2Int ScreenPosToCellNum(Vector2 containerPos, TetrisInventory inventory)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.inventoryRectTransform, containerPos, null, out Vector2 convertPosition);
        return inventory.grid.GetCellNum(convertPosition);
    }

    public void OnDragging(Item_GUI item)
    {
        //Cursor.visible = false;
    }

    public void EndDragging(Item_GUI item)
    {
        Vector3 mousePos = Input.mousePosition;
        Cursor.visible = true;

        //Vector2Int mouseNum = new Vector2Int(0,0);
        //Vector2Int originCellNum = item.GetBelongingCellNum();

        //fromInventoryからデータを削除
        _fromInventory.RemoveItemFromInventory(item.GetBelongingCellNum(), item, item.GetDirection());

        //originCellNum取得のための補正確認
        Vector2Int cellNumOffset = item.GetCellNumRotateOffset(item.GetDirection(), _itemDireciton, _mouseCellNumToOriginCellNumOffset);
        // Debug.Log("回転に合わせた補正 : " + itemDireciton + " , " + cellNumOffset);

        Vector2Int mouseNum = new Vector2Int(0,0);
        Vector2Int originCellNum = item.GetBelongingCellNum();

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

        Debug.Log($"mouseNum{mouseNum}");

        // Debug.Log("入るCell : " + originCellNum);
        int remainNum = 0;

        if (_toInventory != null)
        {
            if(_toInventory.CanPlaceItem(item, originCellNum, _itemDireciton))
            {
                _toInventory.InsertItemToInventory(item, originCellNum, /*item.GetStackNum(), */_itemDireciton/*, out remainNum*/);
                // Debug.Log("おけてはいる");
                Debug.Log("置けた");
                _draggingObject = null;
            }
            else
            {
                originCellNum = item.GetBelongingCellNum();
                _fromInventory.InsertItemToInventory(item, originCellNum, /*item.GetStackNum(), */item.GetDirection()/*, out remainNum*/);
                _draggingObject = null;
                Debug.Log("置けなかった");
            }
        }
        else
        {
            originCellNum = item.GetBelongingCellNum();
            _fromInventory.InsertItemToInventory(item, originCellNum, /*item.GetStackNum(), */item.GetDirection()/*, out remainNum*/);
            _draggingObject = null;
            Debug.Log("toInventoryがなかった");
        }

        //Debug.Log(remainNum);

        // if(remainNum > 0)
        // {
        //     Item_GUI newInstance = InstantiateObject(, remainNum);
        //     _fromInventory.InsertItemToInventory(originCellNum, newInstance, newInstance.GetStackNum(), newInstance.GetDirection(), out remainNum);
        //     Debug.Log("置き直した");
        // }
    }
}
