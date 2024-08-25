using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrisInventorySystem : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private List<TetrisInventory> tetrisInventoriesList = new List<TetrisInventory>();
    private TetrisInventory toInventory;
    private TetrisInventory fromInventory;
    [SerializeField]
    private PlacedObject testDragObject1;

    [SerializeField]
    private PlacedObject testDragObject2;
    private Vector2 dragPositionOffset;
    private Vector2Int dragCellNumOffset;
    private Scriptable_UI_Item.Dir itemDireciton;

    [SerializeField]
    private List<Scriptable_UI_Item> item_Data_List;

    public void Start()
    {
        testDragObject1.OnSetUp(item_Data_List[0]);
        testDragObject1.onBeginDragEvent += StartDragging;
        testDragObject1.onDragEvent += OnDragging;
        testDragObject1.onEndDragEvent += EndDragging;
        tetrisInventoriesList[0].InsertItemToInventory(new Vector2Int(0,0), testDragObject1, testDragObject1.direction);
        

        testDragObject2.OnSetUp(item_Data_List[0]);
        testDragObject2.onBeginDragEvent += StartDragging;
        testDragObject2.onDragEvent += OnDragging;
        testDragObject2.onEndDragEvent += EndDragging;
        tetrisInventoriesList[1].InsertItemToInventory(new Vector2Int(4,5), testDragObject2, testDragObject2.direction);
    }

    public void Update()
    {
        
    }

    void StartDragging(PlacedObject placedObject)
    {
        Vector3 mousePos = Input.mousePosition;
        Cursor.visible = true;
        Debug.Log("掴んだ");
        fromInventory = placedObject.belongingInventory;
        itemDireciton = placedObject.direction;
        //補正用位置取得
        dragPositionOffset = placedObject.rectTransform.position - mousePos;

        //Inventory上でマウス座標を補足し、マウス座標に対応するGrid座標に変換
        Vector2Int mouseNum = ScreenPosToCellNum(mousePos, fromInventory);

        //マウスのGrid座標から現在いるGrid座標を引くことで、マス目補正を取得
        dragCellNumOffset = mouseNum - placedObject.belongingCellNum;

        //親子関係をcanvasに変更（すべてのオブジェクトよりも前にいくことでBackground問題を解決する）
        placedObject.rectTransform.SetParent(canvas.transform);
    }

    #region danger
    Vector2Int ScreenPosToCellNum(Vector2 position, TetrisInventory inventory)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.inventoryRectTransform, position, null, out Vector2 convertPosition);
        return inventory.grid.GetCellNum(convertPosition);
    }
    #endregion

    void OnDragging(PlacedObject placedObject)
    {
        Cursor.visible = true;
        //位置補正
        placedObject.rectTransform.position = Input.mousePosition + new Vector3(dragPositionOffset.x, dragPositionOffset.y, 0);
    }

    void EndDragging(PlacedObject placedObject)
    {
        Vector3 mousePos = Input.mousePosition;
        Cursor.visible = true;

        Vector2Int mouseNum = new Vector2Int(0,0);
        Vector2Int originCellNum = placedObject.belongingCellNum;

        fromInventory.RemoveItemFromInventory(placedObject.belongingCellNum, placedObject, placedObject.direction);

        foreach (TetrisInventory inventory in tetrisInventoriesList) 
        {
            mouseNum = ScreenPosToCellNum(mousePos, inventory);
            originCellNum = mouseNum - dragCellNumOffset;

            if (inventory.grid.IsValidCellNum(originCellNum))
            {
                toInventory = inventory;
                break;
            }
        }
        Debug.Log("mouseNum : " + mouseNum + "offset : " + dragCellNumOffset);
        Debug.Log("originCellNum" + originCellNum);

        if (toInventory != null)
        {
            if(toInventory.TryPlaceItem(placedObject, originCellNum))
            {
                Debug.Log("toInventory : true, TryPlace : true");
                //移動中に回転することを考えていない
                //以前所属していた各セルからオブジェクトを削除
                toInventory.InsertItemToInventory(originCellNum, placedObject, this.itemDireciton);
            }
            else
            {
                Debug.Log("toInventory : true, TryPlace : false");
                originCellNum = placedObject.belongingCellNum;
                fromInventory.InsertItemToInventory(originCellNum, placedObject, placedObject.direction);
            }
        }
        else
        {
            Debug.Log("toInventory : false");
            originCellNum = placedObject.belongingCellNum;
            fromInventory.InsertItemToInventory(originCellNum, placedObject, placedObject.direction);
        }
    }
}
