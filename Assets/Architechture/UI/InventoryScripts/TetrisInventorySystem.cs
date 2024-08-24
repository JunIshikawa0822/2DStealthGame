using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrisInventorySystem : MonoBehaviour
{
    //PlacedObject holdingItem;
    [SerializeField]
    private List<TetrisInventory> tetrisInventoriesList = new List<TetrisInventory>();
    private TetrisInventory toInventory;
    private TetrisInventory fromInventory;

    [SerializeField]
    TetrisInventory testInventory;

    [SerializeField]
    private PlacedObject testDragObject1;

    [SerializeField]
    private PlacedObject testDragObject2;

    // [SerializeField]
    // private RectTransform check;

    private Vector2 dragPositionOffset;
    private Vector2Int dragCellNumOffset;
    //float dragPositionOffset_x;
    //float dragPositionOffset_y;

    //[SerializeField] List<PlacedObject> testItemsList = new List<PlacedObject>();

    public void Start()
    {
        testDragObject1.OnSetUp();
        testDragObject1.onBeginDragEvent += StartDragging;
        testDragObject1.onDragEvent += OnDragging;
        testDragObject1.onEndDragEvent += EndDragging;
        testDragObject1.width = 5;
        testDragObject1.height = 2;

        testInventory.InsertItemToInventory(new Vector2Int(0,0), testDragObject1);
        

        testDragObject2.OnSetUp();
        testDragObject2.onBeginDragEvent += StartDragging;
        testDragObject2.onDragEvent += OnDragging;
        testDragObject2.onEndDragEvent += EndDragging;
        testDragObject2.width = 5;
        testDragObject2.height = 2;

        testInventory.InsertItemToInventory(new Vector2Int(4,5), testDragObject2);
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

        //補正用位置取得
        dragPositionOffset = placedObject.rectTransform.position - mousePos;

        //Inventory上でマウス座標を補足し、マウス座標に対応するGrid座標に変換
        Vector2Int mouseNum = ScreenPosToCellNum(mousePos, fromInventory);

        //マウスのGrid座標から現在いるGrid座標を引くことで、マス目補正を取得
        dragCellNumOffset = mouseNum - placedObject.belongingCellNum;

        //補正用位置取得
        //dragPositionOffset_x = placedObject.rectTransform.position.x - Input.mousePosition.x;
        //dragPositionOffset_y = placedObject.rectTransform.position.y - Input.mousePosition.y;
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
        
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(fromInventory.inventoryRectTransform, placedObject.rectTransform.position, null, out Vector2 convertPosition);
        //placedObject.rectTransform.position = convertPosition;
    }

    void EndDragging(PlacedObject placedObject)
    {
        Vector3 mousePos = Input.mousePosition;
        Cursor.visible = true;

        Vector2Int mouseNum = new Vector2Int(0,0);
        Vector2Int originCellNum = placedObject.belongingCellNum;

        fromInventory.RemoveItemFromInventory(placedObject.belongingCellNum, placedObject);

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
                
                toInventory.InsertItemToInventory(originCellNum, placedObject);
            }
            else
            {
                Debug.Log("toInventory : true, TryPlace : false");
                originCellNum = placedObject.belongingCellNum;
                //Vector2 placedObjectPosition = fromInventory.grid.GetCellOriginAnchoredPosition(originCellNum.x, originCellNum.y); /*+ new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();*/
                // placedObjectTransform.rotation = Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
                //placedObject.rectTransform.anchoredPosition = placedObjectPosition;

                fromInventory.InsertItemToInventory(originCellNum, placedObject);
            }
        }
        else
        {
            Debug.Log("toInventory : false");
            originCellNum = placedObject.belongingCellNum;
            //Vector2 placedObjectPosition = fromInventory.grid.GetCellOriginAnchoredPosition(originCellNum.x, originCellNum.y); /*+ new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();*/
            // placedObjectTransform.rotation = Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
            //placedObject.rectTransform.anchoredPosition = placedObjectPosition;
            fromInventory.InsertItemToInventory(originCellNum, placedObject);
        }
    }
}
