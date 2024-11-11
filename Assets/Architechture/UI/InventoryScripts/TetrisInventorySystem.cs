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
    private CellNumber _mouseCellNumToOriginCellNumOffset;
    //private Vector2Int mouseCellNumToMaxCellNumOffset;
    private Item_GUI _draggingObject;
    private Vector3 _objectPos;
    //private float _originDirectionAngle;
    private float _nextDirectionAngle;
    private float _rotateAngle;

    private CellNumber _oldOriginCellNum;
    private CellNumber _newOriginCellNum;
    private Item_GUI.ItemDir _oldDirection;
    private Item_GUI.ItemDir _newDirection;

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
        Item_GUI instance1 = InstantiateObject(_item_Data_List[0], 5);
        _tetrisInventoriesList[0].InsertItemToInventory(instance1, new CellNumber(0,0), /*instance1.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum1*/);
        
        //test1.anchoredPosition = _tetrisInventoriesList[0].grid.GetCellOriginAnchoredPosition(0, 0);
        Item_GUI instance2 = InstantiateObject(_item_Data_List[0], 1);
        _tetrisInventoriesList[1].InsertItemToInventory(instance2, new CellNumber(4,5), /*instance2.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum2*/);

        Item_GUI instance3 = InstantiateObject(_item_Data_List[0], 3);
        _tetrisInventoriesList[0].InsertItemToInventory(instance3, new CellNumber(0,2), /*instance3.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum3*/);

        Item_GUI instance4 = InstantiateObject(_item_Data_List[0], 2);
        _tetrisInventoriesList[0].InsertItemToInventory(instance4, new CellNumber(0,4), /*instance4.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum4*/);

        Item_GUI instance5 = InstantiateObject(_item_Data_List[0], 1);
        _tetrisInventoriesList[0].InsertItemToInventory(instance5, new CellNumber(0,6), /*instance5.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum5*/);

        Item_GUI instance6 = InstantiateObject(_item_Data_List[0], 4);
        _tetrisInventoriesList[0].InsertItemToInventory(instance6, new CellNumber(0,8), /*instance6.GetStackNum(), */Item_GUI.ItemDir.Down/*, out int remainNum6*/);
    }

    public void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(_tetrisInventoriesList[0].GetContainer(), Input.mousePosition, null, out Vector2 convertPosition);
        //Debug.Log($"セル座標{_tetrisInventoriesList[0].grid.GetCellNum(convertPosition)}");
        // Debug.Log($"mouse座標{convertPosition}");

        if(_draggingObject != null)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {   
                _newDirection = _draggingObject.GetNextDir(_newDirection);//OK
                //Debug.Log(_itemDireciton);
                _nextDirectionAngle = _draggingObject.GetRotationAngle(_newDirection);//OK
                //Debug.Log(_nextDirectionAngle);
                _rotateAngle = _nextDirectionAngle - _draggingObject.GetRotationAngle(_draggingObject.GetDirection());//OK
                //Debug.Log(_rotateAngle);

                _draggingObject.SetRotation(Quaternion.Euler(0, 0, _nextDirectionAngle));

                //掴んだ時(StartDrag)のmouse→anchorPositionのベクトル(_objectPos)を、何度回転させるかという処理
                //あくまで掴んだ時点のベクトルを回転させるため、差分の度数を引数として挿入
                _dragPositionOffset = PositionOffset(_rotateAngle, Vector3.zero, _objectPos);  
            }

            //位置補正
            _draggingObject.SetPosition(Input.mousePosition + _dragPositionOffset);
        }
    }

    private Vector3 PositionOffset(float rotation, Vector3 center, Vector3 vec)
    {
        float x = (vec.x - center.x) * Mathf.Cos(rotation * Mathf.Deg2Rad) - (vec.y - center.y) * Mathf.Sin(rotation * Mathf.Deg2Rad);
        float y = (vec.x - center.x) * Mathf.Sin(rotation * Mathf.Deg2Rad) + (vec.y - center.y) * Mathf.Cos(rotation * Mathf.Deg2Rad);

        Vector3 offsetVec = new Vector3(x, y, 0);
        return offsetVec;
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

        Item_GUI item = GameObject.Instantiate(item_GUI_Prefab, _canvas.transform);
        item.OnSetUp(itemData);
        item.StackingNum = stackNum;

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
        _oldDirection = _newDirection = item.GetDirection();
        //所属inventoryをキャッシュ
        _fromInventory = item.GetBelongingInventory();

        //もし、全てはStackできなかった場合に溢れた分は、もう一度格納し直す必要があるため、キャッシュしとこう
        _oldOriginCellNum = item.GetBelongingCellNum();

        //rotationの補正をリセット
        _rotateAngle = 0;

        #region いる？
        //取得時の回転をキャッシュ
        //_originDirectionAngle = draggingItemData.GetRotationAngle(_itemDireciton);
        #endregion

        Vector3 mousePos = Input.mousePosition;
        Cursor.visible = true;

        //補正用位置取得
        _objectPos = /*Canvasを基本とする座標系*/item.transform.position - /*Screenを基本とする座標系*/mousePos;
        _dragPositionOffset = _objectPos;

        //Inventory上でマウス座標を補足し、マウス座標に対応するGrid座標に変換
        CellNumber mouseCellNum = ScreenPosToCellNum(mousePos, _fromInventory);
        //Debug.Log(mouseCellNum);

        //マウスのGrid座標から現在いるGrid座標を引くことで、マス目補正を取得
        _mouseCellNumToOriginCellNumOffset = mouseCellNum - _oldOriginCellNum;
        Debug.Log($"offset{_mouseCellNumToOriginCellNumOffset}");
        //親子関係をcanvasに変更（すべてのオブジェクトよりも前にいくことでBackground問題を解決する）
        item.transform.SetParent(_canvas.transform);
    }

    private CellNumber ScreenPosToCellNum(Vector2 pos, TetrisInventory inventory)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.GetContainer(), pos, null, out Vector2 convertPosition);
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

        //fromInventoryからデータを削除
        _fromInventory.RemoveItemFromInventory(_oldOriginCellNum, item, _oldDirection);

        //originCellNum取得のための補正確認
        CellNumber cellNumOffset = item.GetRotatedCellNumOffset(item.GetDirection(), _newDirection, _mouseCellNumToOriginCellNumOffset);
        Debug.Log("回転に合わせた補正 : " + cellNumOffset);

        CellNumber mouseNum = new CellNumber(0,0);

        //所属Inventoryを探す
        foreach (TetrisInventory inventory in _tetrisInventoriesList)
        {
            mouseNum = ScreenPosToCellNum(mousePos, inventory);
            _newOriginCellNum = mouseNum - cellNumOffset;

            // Debug.Log($"inventoryName: {inventory}, mouse: {mouseNum}");

            if (inventory.grid.IsValidCellNum(_newOriginCellNum))
            {
                _toInventory = inventory;
                break;
            }
        }
        //Debug.Log($"mouseNum{mouseNum}");
        Debug.Log($"origin{_newOriginCellNum}");

        List<CellNumber> occupyCellNumList = item.GetCellNumList(_newDirection, _newOriginCellNum);

        if (_toInventory != null || item == null)
        {
            if(_toInventory.CanPlaceItem(item, occupyCellNumList))
            {
                Debug.Log("おけてはいる");
                uint remain = _toInventory.InsertItemToInventory(item, _newOriginCellNum, _newDirection);
                //test1.transform.position = item.GetRectTransform().anchoredPosition;
                Debug.Log("置けた");
                
                if(remain > 0)
                {
                    //増やしてfromInventoryに再度格納
                    Item_GUI newInstance = InstantiateObject(item.GetItemData(), remain);
                    _fromInventory.InsertItemToInventory(newInstance, _oldOriginCellNum, _oldDirection);
                }
            }
            else
            {
                _fromInventory.InsertItemToInventory(item, _oldOriginCellNum, _oldDirection);
                
                Debug.Log("置けなかった");
            }
        }
        else
        {
            Debug.Log("toInventory、itemがなかった");
            _fromInventory.InsertItemToInventory(item, _oldOriginCellNum, _oldDirection);
        }

        _draggingObject = null;
    }
}
