using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrisInventorySystem : MonoBehaviour
{
    //PlacedObject holdingItem;
    //[SerializeField]
    //private List<TetrisInventory> tetrisInventoriesList = new List<TetrisInventory>();
    private TetrisInventory toInventory;
    private TetrisInventory fromInventory;

    [SerializeField]
    private PlacedObject dragObject;

    // [SerializeField]
    // private RectTransform check;

    //private Vector2 dragOffset;
    float dragOffset_x;
    float dragOffset_y;

    //[SerializeField] List<PlacedObject> testItemsList = new List<PlacedObject>();

    public void Start()
    {
        dragObject.OnSetUp();
        dragObject.onBeginDragEvent += StartDragging;
        dragObject.onDragEvent += OnDragging;
        dragObject.onEndDragEvent += EndDragging;
        //if(tetrisInventoriesList[0] == null)return;
        //PlacedObject instance = Instantiate(testItemsList[0], tetrisInventoriesList[0].transform);
    }

    void StartDragging(PlacedObject placedObject)
    {
        Cursor.visible = true;
        Debug.Log("掴んだ");
        fromInventory = placedObject.belongingInventory;

        //補正用位置取得
        dragOffset_x = placedObject.rectTransform.position.x - Input.mousePosition.x;
        dragOffset_y = placedObject.rectTransform.position.y - Input.mousePosition.y;
    }

    void OnDragging(PlacedObject placedObject)
    {
        Cursor.visible = false;
        //位置補正
        placedObject.rectTransform.position = Input.mousePosition + new Vector3(dragOffset_x, dragOffset_y, 0);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(fromInventory.inventoryRectTransform, placedObject.rectTransform.position, null, out Vector2 convertPosition);
        placedObject.rectTransform.position = convertPosition;
    }

    void EndDragging(PlacedObject placedObject)
    {
        Cursor.visible = true;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(toInventory.inventoryRectTransform, placedObject.rectTransform.position, null, out Vector2 convertPosition);
        //check.position = convertPosition;

        Debug.Log("スクリーン : " + placedObject.rectTransform.position + ", RectTransform " + convertPosition);

        Vector2Int placedObjectOrigin = toInventory.GetGridPosition(convertPosition);
        // //マウスカーソル
        // Vector3 screenPoint = Input.mousePosition;
        // Vector2Int placedObjectOrigin;
        // Vector2 anchoredPosition;

        // foreach (TetrisInventory inventoryTetris in tetrisInventoriesList) 
        // {
        //     //スクリー座標を基準としているmousePositionを、ItemContainerのローカル座標を基準としなおす
        //     RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryTetris.inventoryRectTransform, screenPoint, null, out anchoredPosition);

        //     // Debug.Log("DragStop");
        //     placedObjectOrigin = inventoryTetris.GetGridPosition(anchoredPosition);
        //     // placedObjectOrigin = placedObjectOrigin - mouseDragGridPositionOffset;

        //     if (inventoryTetris.grid.IsValidGridPosition(placedObjectOrigin)) 
        //     {
        //         toInventory = inventoryTetris;
        //         break;
        //     }
        // }

        // if(toInventory == null)return;
        
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(toInventory.inventoryRectTransform, screenPoint, null, out anchoredPosition);
        // placedObjectOrigin = toInventory.GetGridPosition(anchoredPosition);
        
        // Vector2Int rotationOffset = placedObject.GetRotationOffset(placedObject.direction);

        // //おこうとする
        // if(toInventory.TryPlaceItem(placedObject, placedObjectOrigin))
        // {    
        //     Vector3 placedObjectWorldPosition = toInventory.grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y) * toInventory.grid.gridCellSize;
        //     //所属Inventoryを更新
        //     placedObject.SetGetBelongingInventory = toInventory;
        //     //Inventory内の位置を更新
        //     placedObject.SetGetInventoryOrigin = placedObjectOrigin;    
        //     //座標を設定
        //     placedObject.rectTransform.anchoredPosition = placedObjectWorldPosition;

        //     //fromInventoryのCellObjectから削除
        //     List<Vector2Int> gridPositionListInFromInventory = fromInventory.GetGridPositionList(placedObject.width, placedObject.height, placedObject.SetGetInventoryOrigin, placedObject.direction);
        //     foreach (Vector2Int gridPosition in gridPositionListInFromInventory) 
        //     {
        //         fromInventory.grid.GetGridObject(gridPosition.x, gridPosition.y).SetGetPlacedObject = null;
        //     }

        //     //toInventoryのCellObjectに挿入
        //     List<Vector2Int> gridPositionListInToInventory = toInventory.GetGridPositionList(placedObject.width, placedObject.height, placedObjectOrigin, placedObject.direction);
        //     foreach (Vector2Int gridPosition in gridPositionListInToInventory) 
        //     {
        //         toInventory.grid.GetGridObject(gridPosition.x, gridPosition.y).SetGetPlacedObject = placedObject;
        //     }
        // }
        // else
        // {
        //     Vector3 placedObjectWorldPosition = fromInventory.grid.GetWorldPosition(placedObject.SetGetInventoryOrigin.x, placedObject.SetGetInventoryOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y) * fromInventory.grid.gridCellSize;     
        //     placedObject.rectTransform.anchoredPosition = placedObjectWorldPosition;
        //     //Inventory内の位置の更新なし
        // }
    }
}
