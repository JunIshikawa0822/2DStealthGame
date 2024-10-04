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

    //マウスから左下への補正位置
    private Vector2Int mouseCellNumToOriginCellNumOffset;
    private Vector2Int mouseCellNumToMaxCellNumOffset;

    private Scriptable_UI_Item draggingItemData;

    private Scriptable_UI_Item.ItemDir originDireciton;
    private Scriptable_UI_Item.ItemDir itemDireciton;
    private PlacedObject draggingObject;

    private Vector3 objectVector;

    private float originDirectionAngle;
    private float nextDirectionAngle;

    private float rotationAngle;

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
        PlacedObject instance1 = InstantiatePlacedObject(item_Data_List[0], 5);
        tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,0), instance1, instance1.GetStackNum(), instance1.GetDirection(), out int remainNum1);
        
        PlacedObject instance2 = InstantiatePlacedObject(item_Data_List[0], 5);
        tetrisInventoriesList[1].InsertItemToInventory(new Vector2Int(4,5), instance2, instance2.GetStackNum(), instance2.GetDirection(), out int remainNum2);

        PlacedObject instance3 = InstantiatePlacedObject(item_Data_List[0], 5);
        tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,2), instance3, instance3.GetStackNum(), instance3.GetDirection(), out int remainNum3);

        PlacedObject instance4 = InstantiatePlacedObject(item_Data_List[0], 5);
        tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,4), instance4, instance4.GetStackNum(), instance4.GetDirection(), out int remainNum4);

        PlacedObject instance5 = InstantiatePlacedObject(item_Data_List[0], 5);
        tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,6), instance5, instance5.GetStackNum(), instance5.GetDirection(), out int remainNum5);

        PlacedObject instance6 = InstantiatePlacedObject(item_Data_List[0], 5);
        tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,8), instance6, instance6.GetStackNum(), instance6.GetDirection(), out int remainNum6);
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
                itemDireciton = draggingItemData.GetNextDir(itemDireciton);
                nextDirectionAngle = draggingItemData.GetRotationAngle(itemDireciton);
                rotationAngle = nextDirectionAngle - originDirectionAngle;
                // Debug.Log(rotationAngle);

                draggingObject.GetRectTransform().rotation = Quaternion.Euler(0, 0, nextDirectionAngle);

                //掴んだ時(StartDrag)のmouse→anchorPositionのベクトルを、何度回転させるかという処理
                //あくまで掴んだ時点のベクトルを回転させるため、差分の度数を引数として挿入
                dragPositionOffset = PositionOffset(rotationAngle, Vector3.zero, objectVector);

                Debug.Log("補正" + mouseCellNumToOriginCellNumOffset);
                Vector2Int offset = draggingItemData.GetCellNumRotateOffset(originDireciton, itemDireciton, mouseCellNumToOriginCellNumOffset);
                //Debug.Log(offset);
            }

            // if(Input.GetMouseButtonDown(1))
            // {
            //     Vector3 mousePos = Input.mousePosition;
            //     Vector2Int mouseNum = new Vector2Int(0,0);
            //     Vector2Int originCellNum = draggingObject.GetBelongingCellNum();

            //     Vector2Int cellNumOffset = draggingItemData.GetCellNumRotateOffset(originDireciton, itemDireciton, mouseCellNumToOriginCellNumOffset);

            //     foreach (TetrisInventory inventory in tetrisInventoriesList)
            //     {
            //         mouseNum = ScreenPosToCellNum(mousePos, inventory);
            //         originCellNum = mouseNum - cellNumOffset;

            //         if (inventory.grid.IsValidCellNum(originCellNum))
            //         {
            //             toInventory = inventory;
            //             break;
            //         }
            //     }

            //     if (toInventory != null)
            //     {    
            //         if(toInventory.CanPlaceItem(draggingObject, originCellNum, itemDireciton))
            //         {
            //             toInventory.InsertItemToInventory(originCellNum, draggingObject, 1, itemDireciton, out int remainNum);
            //             if(remainNum < 1)
            //             {
            //                 draggingObject = null;
            //             }
            //         }
            //     }
            // }

            //位置補正
            draggingObject.GetRectTransform().position = Input.mousePosition + dragPositionOffset;
            //Test(draggingObject);
        }
    }

    private void Test(PlacedObject placedObject)
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2Int mousePosNum = ScreenPosToCellNum(mousePos, fromInventory);

        Vector2Int offset = draggingItemData.GetCellNumRotateOffset(originDireciton, itemDireciton, mouseCellNumToOriginCellNumOffset);

        Vector2Int originCellNum = mousePosNum - offset;

        Debug.Log("mousePos : " + mousePosNum +  " , originPos : " + originCellNum + ", " + itemDireciton);
    }

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

        Transform instance = Instantiate(itemData.prefab, canvas.transform);
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
        draggingObject = placedObject;
        //itemの方向をキャッシュ
        itemDireciton = placedObject.GetDirection();
        originDireciton = itemDireciton;
        //所属inventoryをキャッシュ
        fromInventory = placedObject.GetBelongingInventory();
        //itemDataをキャッシュ
        draggingItemData = placedObject.GetItemData();
        //rotationの補正をリセット
        rotationAngle = 0;
        //取得時の回転をキャッシュ
        originDirectionAngle = draggingItemData.GetRotationAngle(itemDireciton);

        Vector3 mousePos = Input.mousePosition;
        Cursor.visible = true;
        
        //補正用位置取得
        objectVector = /*Canvasを基本とする座標系*/placedObject.GetRectTransform().position - /*Screenを基本とする座標系*/mousePos;
        dragPositionOffset = objectVector;

        //Inventory上でマウス座標を補足し、マウス座標に対応するGrid座標に変換
        Vector2Int mouseNum = ScreenPosToCellNum(mousePos, fromInventory);

        //マウスのGrid座標から現在いるGrid座標を引くことで、マス目補正を取得
        mouseCellNumToOriginCellNumOffset = mouseNum - placedObject.GetBelongingCellNum();
        //親子関係をcanvasに変更（すべてのオブジェクトよりも前にいくことでBackground問題を解決する）
        placedObject.GetRectTransform().SetParent(canvas.transform);
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

        fromInventory.RemoveItemFromInventory(placedObject.GetBelongingCellNum(), placedObject, placedObject.GetDirection());

        //originCellNum取得のための補正確認
        Vector2Int cellNumOffset = draggingItemData.GetCellNumRotateOffset(originDireciton, itemDireciton, mouseCellNumToOriginCellNumOffset);
        // Debug.Log("回転に合わせた補正 : " + itemDireciton + " , " + cellNumOffset);

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

        // Debug.Log("入るCell : " + originCellNum);
        int remainNum = 0;

        if (toInventory != null)
        {
            if(toInventory.CanPlaceItem(placedObject, originCellNum, itemDireciton))
            {
                toInventory.InsertItemToInventory(originCellNum, placedObject, placedObject.GetStackNum(), itemDireciton, out remainNum);
                // Debug.Log("おけてはいる");
                Debug.Log("置けた");
                draggingObject = null;
            }
            else
            {
                originCellNum = placedObject.GetBelongingCellNum();
                fromInventory.InsertItemToInventory(originCellNum, placedObject, placedObject.GetStackNum(), placedObject.GetDirection(), out remainNum);
                draggingObject = null;
                Debug.Log("置けなかった");
            }
        }
        else
        {
            originCellNum = placedObject.GetBelongingCellNum();
            fromInventory.InsertItemToInventory(originCellNum, placedObject, placedObject.GetStackNum(), placedObject.GetDirection(), out remainNum);
            draggingObject = null;
            Debug.Log("toInventoryがなかった");
        }

        //Debug.Log(remainNum);

        if(remainNum > 0)
        {
            PlacedObject newInstance = InstantiatePlacedObject(draggingItemData, remainNum);
            fromInventory.InsertItemToInventory(originCellNum, newInstance, newInstance.GetStackNum(), newInstance.GetDirection(), out remainNum);
            Debug.Log("置き直した");
        }
    }
}
