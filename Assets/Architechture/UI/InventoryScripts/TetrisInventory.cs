using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TetrisInventory : MonoBehaviour
{
    public Grid<CellObject> grid;

    [SerializeField]
    private RectTransform container;

    [SerializeField]
    private GridLayoutGroup background;

    public RectTransform inventoryRectTransform;

    public RectTransform test1;
    public RectTransform test2;
    public RectTransform test3;

    //public GameObject testObject;

    [SerializeField]
    private float cellSize = 50;
    [SerializeField] 
    private int gridWidth = 10;
    [SerializeField]
    private int gridHeight = 10;

    void Awake()
    {
        inventoryRectTransform = this.GetComponent<RectTransform>();
        //Debug.Log(inventoryRectTransform.position);

        //inventoryRectTransform.position = new Vector3(0, 0, 0);
        //test1.position = inventoryRectTransform.position;
        //Debug.Log(test1.position);
        // test2.position = inventoryRectTransform.anchoredPosition;
        // Debug.Log(test2.position);
        // test3.position = inventoryRectTransform.localPosition;
        // Debug.Log(test3.position);

        grid = new Grid<CellObject>
        (
            gridWidth,
            gridHeight,
            cellSize,
            inventoryRectTransform.position,
            //TetrisInventoryの基準点からどれだけ離れた場所に生成するか
            //new Vector2(0, 0),
            //「情報を受け取ってCellObjectを生成する処理」を引き渡す
            (Grid<CellObject> grid, int cellPosition_x, int cellPosition_y) => new CellObject(cellPosition_x, cellPosition_y)
        );

        BackGroundSetUp();
    }

    void Start()
    {
        
        //testObject.transform.position = GetWorldPositionFromRectPosition(test.position, test);
    }

    void Update()
    {
        //grid.ShowDebug(inventoryRectTransform);
    }

    void BackGroundSetUp()
    {
        background.cellSize = new Vector2(cellSize, cellSize);
    }

    public bool TryPlaceItem(PlacedObject placedObject, Vector2Int originCellNum)
    {   
        //オブジェクトが占有するマス目を計算
        List<Vector2Int> gridPositionList = GetCellNumList(originCellNum, placedObject.width, placedObject.height, placedObject.direction);
        //Debug.Log(string.Join(",", gridPositionList));
        bool canPlace = true;

        foreach (Vector2Int gridPosition in gridPositionList) 
        {
            bool isValidPosition = grid.IsValidCellNum(gridPosition);

            if (!isValidPosition)
            {
                // Not valid
                canPlace = false;
                break;
            }

            //Cellがすでに埋まっている場合
            if(grid.GetGridObject(gridPosition.x, gridPosition.y).SetGetPlacedObject != null)
            {
                canPlace = false;
                break;
            }
        }

        if (canPlace) 
        {
            // Object Placed!
            return true;
        } 
        else 
        {
            // Object CANNOT be placed!
            return false;
        }
    }

    public List<Vector2Int> GetCellNumList(Vector2Int originCellNum, int objectWidth, int objectHeight, PlacedObject.Dir dir) 
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        switch (dir) 
        {
            default:
            case PlacedObject.Dir.Down:
            case PlacedObject.Dir.Up:
                for (int x = 0; x < objectWidth; x++) {
                    for (int y = 0; y < objectHeight; y++) {
                        gridPositionList.Add(originCellNum + new Vector2Int(x, y));
                    }
                }
                break;
            case PlacedObject.Dir.Left:
            case PlacedObject.Dir.Right:
                for (int x = 0; x < objectHeight; x++) {
                    for (int y = 0; y < objectWidth; y++) {
                        gridPositionList.Add(originCellNum + new Vector2Int(x, y));
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
        }
    }

    public void InsertItemToInventory(Vector2Int originCellNum, PlacedObject placedObject)
    {
        List<Vector2Int> cellNumList = GetCellNumList(originCellNum, placedObject.width, placedObject.height, placedObject.direction);
        foreach (Vector2Int cellNum in cellNumList)
        {
            //Debug.Log(cellNum);
            grid.GetGridObject(cellNum.x, cellNum.y).SetGetPlacedObject = placedObject;
        }

        //placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.belongingCellNum = originCellNum;
        Debug.Log(placedObject.belongingCellNum);
        placedObject.belongingInventory = this;
        placedObject.rectTransform.SetParent(container);

        Vector2 placedObjectAnchoredPosition = grid.GetCellOriginAnchoredPosition(originCellNum.x, originCellNum.y); /*+ new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();*/
        //Debug.Log(placedObjectPosition);
        // placedObjectTransform.rotation = Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        placedObject.rectTransform.anchoredPosition = placedObjectAnchoredPosition;
        //test2.anchoredPosition = placedObjectPosition;

        //アイテムの比率をInventoryごとに変えられる
        placedObject.ImageSizeSet(cellSize * placedObject.width, cellSize * placedObject.height);

        CheckCellObject();
    }

    public void RemoveItemFromInventory(Vector2Int originCellNum, PlacedObject placedObject)
    {
        List<Vector2Int> removeCellNumList = GetCellNumList(originCellNum, placedObject.width, placedObject.height, placedObject.direction);
        foreach (Vector2Int cellNum in removeCellNumList)
        {
            grid.GetGridObject(cellNum.x, cellNum.y).SetGetPlacedObject = null;
        }

        placedObject.rectTransform.parent = null;
    }

    public void CheckCellObject()
    {
        List<Vector2Int> testlist = new List<Vector2Int>();

        Debug.Log("チェック開始");
        for(int i = 0; i < gridWidth; i++)
        {
            for(int j = 0; j < gridHeight; j++)
            {
                if(grid.GetGridObject(i, j).SetGetPlacedObject == null) continue;
                testlist.Add(new Vector2Int(i, j));
                //Debug.Log("(" + i + "," + j + ") : " + grid.GetGridObject(i, j).SetGetPlacedObject);     
            }
        }

        Debug.Log(string.Join("," , testlist));
        Debug.Log("チェック終了");
    }
}
