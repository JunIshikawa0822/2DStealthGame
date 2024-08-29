using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TetrisInventorySystem : MonoBehaviour, IOnUpdate
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private List<TetrisInventory> tetrisInventoriesList = new List<TetrisInventory>();
    private TetrisInventory toInventory;
    private TetrisInventory fromInventory;

    private Vector3 dragPositionOffset;
    private Vector2Int dragCellNumOffset;
    
    private Scriptable_UI_Item.ItemDir itemDireciton;
    private PlacedObject draggingObject;

    private Vector3 objectVector;

    private float rotationOffset;

    [SerializeField]
    private List<Scriptable_UI_Item> item_Data_List;

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
        PlacedObject instance1 = InstantiatePlacedObject(item_Data_List[0], 1);
        tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,0), instance1, instance1.GetDirection(), out Vector2 pos1);
        
        PlacedObject instance2 = InstantiatePlacedObject(item_Data_List[0], 1);
        tetrisInventoriesList[1].InsertItemToInventory(new Vector2Int(4,5), instance2, instance2.GetDirection(),out Vector2 pos2);

        PlacedObject instance3 = InstantiatePlacedObject(item_Data_List[0], 1);
        tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,2), instance3, instance3.GetDirection(), out Vector2 pos3);

        PlacedObject instance4 = InstantiatePlacedObject(item_Data_List[0], 1);
        tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,4), instance4, instance4.GetDirection(), out Vector2 pos4);

        PlacedObject instance5 = InstantiatePlacedObject(item_Data_List[0], 1);
        tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,6), instance5, instance5.GetDirection(), out Vector2 pos5);

        PlacedObject instance6 = InstantiatePlacedObject(item_Data_List[0], 1);
        tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,8), instance6, instance6.GetDirection(), out Vector2 pos6);
    }

    public void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
         if(draggingObject != null)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                rotationOffset += 90f;

                itemDireciton = draggingObject.GetItemData().GetNextDir(itemDireciton);
                draggingObject.GetRectTransform().rotation = Quaternion.Euler(0, 0, draggingObject.GetItemData().GetRotationAngle(itemDireciton));
                dragPositionOffset = PositionOffset(rotationOffset, Vector3.zero, objectVector);

                Vector2Int offset = draggingObject.GetItemData().GetDragCellNumRotateOffset(itemDireciton, dragCellNumOffset);
                Debug.Log(offset);
            }
            //位置補正
            draggingObject.GetRectTransform().position = Input.mousePosition + dragPositionOffset;

            Test(draggingObject);
        }
    }

    private void Test(PlacedObject placedObject)
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2Int mousePosNum = ScreenPosToCellNum(mousePos, fromInventory);

        Vector2Int offset = placedObject.GetItemData().GetDragCellNumRotateOffset(itemDireciton, dragCellNumOffset);

        Vector2Int originCellNum = mousePosNum - offset;

        Debug.Log(originCellNum);
    }

#region retouching

    private Vector3 PositionOffset(float rotation, Vector3 center, Vector3 vec)
    {
        float x = (vec.x - center.x) * Mathf.Cos(rotation * Mathf.Deg2Rad) - (vec.y - center.y) * Mathf.Sin(rotation * Mathf.Deg2Rad);
        float y = (vec.x - center.x) * Mathf.Sin(rotation * Mathf.Deg2Rad) + (vec.y - center.y) * Mathf.Cos(rotation * Mathf.Deg2Rad);

        Vector3 offsetVec = new Vector3(x, y, 0);
        //Debug.Log(offsetVec.magnitude);
        return offsetVec;
    }

#endregion

    private PlacedObject InstantiatePlacedObject(Scriptable_UI_Item itemData, int stackNum)
    {
        //Debug.Log(itemData);
        Transform instance = Instantiate(itemData.prefab, canvas.transform);
        PlacedObject placedObject = instance.GetComponent<PlacedObject>();
        placedObject.OnSetUp(itemData, stackNum);

        placedObject.onBeginDragEvent += StartDragging;
        placedObject.onDragEvent += OnDragging;
        placedObject.onEndDragEvent += EndDragging;

        return placedObject;
    }

    public void StartDragging(PlacedObject placedObject)
    {
        placedObject.OnDragStart();
        //ドラッグしているオブジェクトをキャッシュ
        draggingObject = placedObject;
        //itemの方向をキャッシュ
        itemDireciton = placedObject.GetDirection();
        //所属inventoryをキャッシュ
        fromInventory = placedObject.GetBelongingInventory();
        //rotationの補正をリセット
        rotationOffset = 0;

        Vector3 mousePos = Input.mousePosition;
        Cursor.visible = true;
        
        //補正用位置取得
        objectVector = /*Canvasを基本とする座標系*/placedObject.GetRectTransform().position - /*Screenを基本とする座標系*/mousePos;
        dragPositionOffset = objectVector;
        //Debug.Log(dragPositionOffset);

        //Inventory上でマウス座標を補足し、マウス座標に対応するGrid座標に変換
        Vector2Int mouseNum = ScreenPosToCellNum(mousePos, fromInventory);
        

        //マウスのGrid座標から現在いるGrid座標を引くことで、マス目補正を取得
        dragCellNumOffset = mouseNum - placedObject.GetBelongingCellNum();
        Debug.Log("補正取得 : " + dragCellNumOffset);

        //親子関係をcanvasに変更（すべてのオブジェクトよりも前にいくことでBackground問題を解決する）
        placedObject.GetRectTransform().SetParent(canvas.transform);
    }

    #region danger
    private Vector2Int ScreenPosToCellNum(Vector2 position, TetrisInventory inventory)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.inventoryRectTransform, position, null, out Vector2 convertPosition);
        return inventory.grid.GetCellNum(convertPosition);
    }
    #endregion

    public void OnDragging(PlacedObject placedObject)
    {
        Cursor.visible = false;
    }

    public void EndDragging(PlacedObject placedObject)
    {
        placedObject.OnDragEnd();

        Vector3 mousePos = Input.mousePosition;
        Cursor.visible = true;

        Vector2Int mouseNum = new Vector2Int(0,0);
        Vector2Int originCellNum = placedObject.GetBelongingCellNum();

        fromInventory.RemoveItemFromInventory(placedObject.GetBelongingCellNum(), placedObject, placedObject.GetDirection());

        //originCellNum取得のための補正確認
        Vector2Int cellNumOffset = placedObject.GetItemData().GetDragCellNumRotateOffset(itemDireciton, dragCellNumOffset);
        Debug.Log("回転に合わせた補正 : " + itemDireciton + " , " + cellNumOffset);

        //所属Inventoryを探す
        foreach (TetrisInventory inventory in tetrisInventoriesList)
        {
            mouseNum = ScreenPosToCellNum(mousePos, inventory);
            originCellNum = mouseNum - cellNumOffset;

            if (inventory.grid.IsValidCellNum(originCellNum))
            {
                toInventory = inventory;
                break;
            }
        }

        if (toInventory != null)
        {
            if(toInventory.TryPlaceItem(placedObject, originCellNum, itemDireciton))
            {
                Debug.Log("toInventory : true, TryPlace : true");
                //移動中に回転することを考えていない

                Debug.Log("dragging : " + itemDireciton);
                //Debug.Log("placing : " + placedObject.direction);
                //以前所属していた各セルからオブジェクトを削除
                toInventory.InsertItemToInventory(originCellNum, placedObject, itemDireciton, out Vector2 pos);
                //test1.GetComponent<RectTransform>().position = pos;
            }
            else
            {
                Debug.Log("toInventory : true, TryPlace : false");

                Debug.Log("dragging : " + itemDireciton);
                Debug.Log("placing : " + placedObject.GetDirection());
                originCellNum = placedObject.GetBelongingCellNum();
                fromInventory.InsertItemToInventory(originCellNum, placedObject, placedObject.GetDirection(), out Vector2 pos);
            }
        }
        else
        {
            Debug.Log("toInventory : false");

            Debug.Log("dragging : " + itemDireciton);
            Debug.Log("placing : " + placedObject.GetDirection());
            originCellNum = placedObject.GetBelongingCellNum();
            fromInventory.InsertItemToInventory(originCellNum, placedObject, placedObject.GetDirection(), out Vector2 pos);
        }

        draggingObject = null;
    }
}
