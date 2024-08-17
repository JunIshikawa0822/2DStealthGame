using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisInventory : MonoBehaviour
{
    private Grid<CellObject> grid;

    [SerializeField] RectTransform UI;
    //[SerializeField] GameObject gameObject;

    [SerializeField] float cellSize;
    [SerializeField] int gridWidth = 10;
    [SerializeField] int gridHeight = 10;

    PlacedObject holdingItem;

    void Awake()
    {
        grid = new Grid<CellObject>
        (
            gridWidth, 
            gridHeight, 
            cellSize, 
            UI.position,

            //「情報を受け取ってCellObjectを生成する処理」を引き渡す
            (Grid<CellObject> grid, int cellPosition_x, int cellPosition_y) => new CellObject(grid, cellPosition_x, cellPosition_y)
        );

        Debug.Log(UI.position);
    }

    void Start()
    {
        holdingItem.OnBeginDragEvent += StartDragging;
        holdingItem.OnDragEvent += OnDragging;
        holdingItem.OnEndDragEvent += EndDragging;
    }

    void Update()
    {
        
    }

    void EndDrag()
    {
        //foreach (InventoryTetris inventoryTetris in inventoryTetrisList) {
            // Vector3 screenPoint = Input.mousePosition;

            // //スクリーン座標を基準としているmousePositionを、ItemContainerのワールド座標を基準としなおす
            // RectTransformUtility.ScreenPointToWorldPointInRectangle(UI, screenPoint, null, out Vector3 anchoredPosition);

            // Vector2Int placedObjectOrigin = GetGridPosition(anchoredPosition);
            // Debug.Log(placedObjectOrigin);
            //placedObjectOrigin = placedObjectOrigin - mouseDragGridPositionOffset;

            // if (inventoryTetris.IsValidGridPosition(placedObjectOrigin)) {
            //     toInventoryTetris = inventoryTetris;
            //     break;
            // }
        //}
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition) 
    {
        Debug.Log("GetXY!!!!!!!!!!!!!");
        GetXY(worldPosition, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) 
    {  
        x = Mathf.FloorToInt((worldPosition.x - UI.position.x) / cellSize);
        y = Mathf.FloorToInt((worldPosition.y - UI.position.y) / cellSize);
        //Debug.Log("x : " + x + ", y : " + y);
        //Debug.Log("cellSize : " + cellSize + " , worldPosition : " + worldPosition + " , originPosition : " + originPosition);
    }

    public bool TryPlaceItem(PlacedObject placedObject, Vector2Int placedObjectOrigin) {
        // Test Can Build
        //Debug.Log("placeObjectOrigin : " + placedObjectOrigin);
        
        List<Vector2Int> gridPositionList = GetGridPositionList(placedObject.width, placedObject.height, placedObjectOrigin, placedObject.direction);
        //Debug.Log(string.Join(",", gridPositionList));
        bool canPlace = true;

        foreach (Vector2Int gridPosition in gridPositionList) 
        {
            bool isValidPosition = grid.IsValidGridPosition(gridPosition);

            if (!isValidPosition)
            {
                // Not valid
                canPlace = false;
                break;
            }

            bool isOccupied = grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild();

            if (isOccupied == false) 
            {
                canPlace = false;
                break;
            }
        }

        if (canPlace) 
        {
            //OnObjectPlaced?.Invoke(this, placedObject);

            // Object Placed!
            return true;
        } 
        else 
        {
            // Object CANNOT be placed!
            return false;
        }
    }

    // public void StoppedDragging(TetrisInventory fromInventoryTetris, PlacedObject placedObject) {
    //     draggingInventoryTetris = null;
    //     draggingPlacedObject = null;

    //     Cursor.visible = true;

    //     // Remove item from its current inventory
    //     fromInventoryTetris.RemoveItemAt(placedObject.GetGridPosition());

    //     TetrisInventory toInventoryTetris = null;

    //     // Find out which InventoryTetris is under the mouse position
    //     // foreach (InventoryTetris inventoryTetris in inventoryTetrisList) {
    //     //     Vector3 screenPoint = Input.mousePosition;

    //     //     //スクリー座標を基準としているmousePositionを、ItemContainerのローカル座標を基準としなおす
    //     //     RectTransformUtility.ScreenPointToLocalPointInRectangle(UI, screenPoint, null, out Vector2 anchoredPosition);

    //     //     Debug.Log("DragStop");
    //     //     Vector2Int placedObjectOrigin = inventoryTetris.GetGridPosition(anchoredPosition);

    //     //     if (inventoryTetris.IsValidGridPosition(placedObjectOrigin)) 
    //     //     {
    //     //         toInventoryTetris = inventoryTetris;
    //     //         break;
    //     //     }
    //     // }

    //     // Check if it's on top of a InventoryTetris
    //     if (toInventoryTetris != null) {
    //         Vector3 screenPoint = Input.mousePosition;
    //         RectTransformUtility.ScreenPointToWorldPointInRectangle(UI, screenPoint, null, out Vector3 anchoredPosition);
    //         Vector2Int placedObjectOrigin = toInventoryTetris.GetGridPosition(anchoredPosition);

    //         bool tryPlaceItem = toInventoryTetris.TryPlaceItem(placedObject.GetPlacedObjectTypeSO() as ItemTetrisSO, placedObjectOrigin, dir);

    //         if (tryPlaceItem) 
    //         {
    //             // Item placed!
    //         } 
    //         else 
    //         {
    //             // Cannot drop item here!
    //             TooltipCanvas.ShowTooltip_Static("Cannot Drop Item Here!");
    //             FunctionTimer.Create(() => { TooltipCanvas.HideTooltip_Static(); }, 2f, "HideTooltip", true, true);

    //             // Drop on original position
    //             fromInventoryTetris.TryPlaceItem(placedObject.GetPlacedObjectTypeSO() as ItemTetrisSO, placedObject.GetGridPosition(), placedObject.GetDir());
    //         }
    //     } else {
    //         // Not on top of any Inventory Tetris!

    //         // Cannot drop item here!
    //         TooltipCanvas.ShowTooltip_Static("Cannot Drop Item Here!");
    //         FunctionTimer.Create(() => { TooltipCanvas.HideTooltip_Static(); }, 2f, "HideTooltip", true, true);

    //         // Drop on original position
    //         fromInventoryTetris.TryPlaceItem(placedObject.GetPlacedObjectTypeSO() as ItemTetrisSO, placedObject.GetGridPosition(), placedObject.GetDir());
    //     }
    // }

    // public void StartedDragging(InventoryTetris inventoryTetris, PlacedObject placedObject) {
    //     // Started Dragging
    //     draggingInventoryTetris = inventoryTetris;
    //     draggingPlacedObject = placedObject;

    //     Cursor.visible = false;

    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryTetris.GetItemContainer(), Input.mousePosition, null, out Vector2 anchoredPosition);
    //     Vector2Int mouseGridPosition = inventoryTetris.GetGridPosition(anchoredPosition);

    //     // Calculate Grid Position offset from the placedObject origin to the mouseGridPosition
    //     mouseDragGridPositionOffset = mouseGridPosition - placedObject.GetGridPosition();

    //     // Calculate the anchored poisiton offset, where exactly on the image the player clicked
    //     mouseDragAnchoredPositionOffset = anchoredPosition - placedObject.GetComponent<RectTransform>().anchoredPosition;

    //     // Save initial direction when started draggign
    //     dir = placedObject.GetDir();

    //     // Apply rotation offset to drag anchored position offset
    //     Vector2Int rotationOffset = draggingPlacedObject.GetPlacedObjectTypeSO().GetRotationOffset(dir);
    //     mouseDragAnchoredPositionOffset += new Vector2(rotationOffset.x, rotationOffset.y) * draggingInventoryTetris.GetGrid().GetCellSize();
    // }

    void StartDragging()
    {
        Cursor.visible = false;

        // if(item == null && item.GetType() == typeof(PlacedObject))
        // holdingItem = item;
    }

    void OnDragging()
    {
        Vector3 screenPoint = Input.mousePosition;
        //スクリーン座標を基準としているmousePositionを、ItemContainerのワールド座標を基準としなおす
        RectTransformUtility.ScreenPointToWorldPointInRectangle(UI, screenPoint, null, out Vector3 anchoredPosition);
        holdingItem.transform.position = anchoredPosition;
    }

    void EndDragging()
    {
        Cursor.visible = true;

        Vector3 screenPoint = Input.mousePosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(UI, screenPoint, null, out Vector3 anchoredPosition);
        Vector2Int placedObjectOrigin = GetGridPosition(anchoredPosition);

         List<Vector2Int> gridPositionList = GetGridPositionList(holdingItem.width, holdingItem.height, placedObjectOrigin, placedObject.direction);

        if(TryPlaceItem(holdingItem, placedObjectOrigin))
        {
            Vector2Int rotationOffset = holdingItem.GetRotationOffset(holdingItem.direction);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.gridCellSize;
            holdingItem.rectTransform.anchoredPosition = placedObjectWorldPosition;
            //PlacedObject placedObject = PlacedObject.CreateCanvas(itemContainer, placedObjectWorldPosition, placedObjectOrigin, dir, itemTetrisSO);
            //holdingItem.transform.rotation = Quaternion.Euler(0, 0, -itemTetrisSO.GetRotationAngle(dir));

            //placedObject.GetComponent<InventoryTetrisDragDrop>().Setup(this);

            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(holdingItem);
            }
        }

    }

    public List<Vector2Int> GetGridPositionList(int width, int height, Vector2Int offset, PlacedObject.Dir dir) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir) {
            default:
            case PlacedObject.Dir.Down:
            case PlacedObject.Dir.Up:
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        gridPositionList.Add(/*offset + */new Vector2Int(x, y));
                    }
                }
                break;
            case PlacedObject.Dir.Left:
            case PlacedObject.Dir.Right:
                for (int x = 0; x < height; x++) {
                    for (int y = 0; y < width; y++) {
                        gridPositionList.Add(/*offset + */new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }
}
