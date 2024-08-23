using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TetrisInventory : MonoBehaviour
{
    public Grid<CellObject> grid;

    [SerializeField]
    private Transform container;

    [SerializeField]
    private Transform backGroundContainer;

    [SerializeField]
    private Image backGroundTileImage;

    [SerializeField] 
    public RectTransform inventoryRectTransform;
    //[SerializeField] GameObject gameObject;

    public RectTransform test;

    public GameObject testObject;

    [SerializeField]
    private float cellSize = 50;
    [SerializeField] 
    private int gridWidth = 10;
    [SerializeField] 
    private int gridHeight = 10;

    //public event Action<TetrisInventory> onPointerEnterEvent;

    void Awake()
    {
        //Debug.Log(inventoryRectTransform.position);

        //Debug.Log("origin" + inventoryRectTransform.position);
        grid = new Grid<CellObject>
        (
            gridWidth,
            gridHeight,
            cellSize,
            inventoryRectTransform.position,
            //「情報を受け取ってCellObjectを生成する処理」を引き渡す
            (Grid<CellObject> grid, int cellPosition_x, int cellPosition_y) => new CellObject(grid, cellPosition_x, cellPosition_y, grid.GetWorldPosition(cellPosition_x, cellPosition_y))
        );

        //ShowDebug(inventoryRectTransform.anchoredPosition);
    }

    void Start()
    {
        
        //testObject.transform.position = GetWorldPositionFromRectPosition(test.position, test);
    }

    // public void ShowDebug(Vector3 pos)
    // {
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryRectTransform, pos, null, out Vector2 result);
    //     Debug.Log(result);
    //     testObject.transform.position = result;
    // }

    Vector3 GetWorldPositionFromRectPosition(Vector2 pos, RectTransform rect)
    {
        //UI座標からスクリーン座標に変換
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, pos);
        //ワールド座標
        Vector3 result = Vector3.zero;
        //スクリーン座標→ワールド座標に変換
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPos, null, out result);
        return result;
    }

    void Update()
    {
        //grid.ShowDebug(inventoryRectTransform);
    }

    void CreateBackGround()
    {
        GridLayoutGroup gridLayoutGroup = backGroundContainer.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        // Vector2 size = new Vector2(cellSize, cellSize);
        foreach(CellObject cellObject in grid.gridArray)
        {
            Instantiate(backGroundTileImage, cellObject.GetCellObjectPosition, Quaternion.identity, backGroundContainer);
            //instance.rectTransform.sizeDelta = celsize;
        }
    }

    public Vector2Int GetGridPosition(Vector2 rectTransformPosition)
    {
        int x = Mathf.FloorToInt((rectTransformPosition.x - inventoryRectTransform.position.x) / cellSize);
        int y = Mathf.FloorToInt((rectTransformPosition.y - inventoryRectTransform.position.y) / cellSize);

        return new Vector2Int(x, y);
    }

    public bool TryPlaceItem(PlacedObject placedObject, Vector2Int placedObjectOrigin) 
    {
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

    public void ItemLoadInInventory(List<PlacedObject> objectList)
    {
        foreach (PlacedObject placedObject in objectList)
        {
            PlacedObject instance = Instantiate(placedObject, container);
            instance.OnSetUp();

            // Vector2Int placeOrigin = new Vector2Int(5, 5);

            // if(TryPlaceItem(instance, placeOrigin))
            // {
            //     Vector2Int rotationOffset = placedObject.GetRotationOffset(instance.direction);
            //     Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placeOrigin.x, placeOrigin.y); /*+ new Vector3(rotationOffset.x, rotationOffset.y) * grid.gridCellSize;*/
            //     //所属Inventoryを更新
            //     instance.SetGetBelongingInventory = this;
            //     //Inventory内の位置を更新
            //     instance.SetGetInventoryOrigin = placeOrigin;    
            //     //座標を設定
            //     instance.rectTransform.anchoredPosition = placedObjectWorldPosition;

            //     List<Vector2Int> gridPositionListInToInventory = GetGridPositionList(instance.width, instance.height, placeOrigin, instance.direction);
            //     foreach (Vector2Int gridPosition in gridPositionListInToInventory) 
            //     {
            //         grid.GetGridObject(gridPosition.x, gridPosition.y).SetGetPlacedObject = instance;
            //     }
            // }
            // else
            // {
            //     Destroy(instance);
            // }

            //Debug.Log(grid.GetGridObject(placeOrigin.x, placeOrigin.y).SetGetPlacedObject);
        }
    }
}
