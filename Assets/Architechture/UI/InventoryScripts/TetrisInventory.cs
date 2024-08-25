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
    private RectTransform background;

    public RectTransform inventoryRectTransform;

    // public RectTransform test1;
    // public RectTransform test2;
    // public RectTransform test3;

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

        grid = new Grid<CellObject>
        (
            gridWidth,
            gridHeight,
            cellSize,
            //inventoryRectTransform.position,
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
        background.sizeDelta = new Vector2(gridWidth, gridHeight) * cellSize;
        background.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
    }

    public bool TryPlaceItem(PlacedObject placedObject, Vector2Int originCellNum)
    {   
        //オブジェクトが占有するマス目を計算
        List<Vector2Int> gridPositionList = placedObject.GetItemData().GetCellNumList(originCellNum, placedObject.direction);
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

    // public List<Vector2Int> GetCellNumList(Vector2Int originCellNum, int objectWidth, int objectHeight, Scriptable_UI_Item.Dir dir) 
    // {
    //     List<Vector2Int> gridPositionList = new List<Vector2Int>();

    //     switch (dir)
    //     {
    //         default:
    //         case Scriptable_UI_Item.Dir.Down:
    //         case Scriptable_UI_Item.Dir.Up:
    //             for (int x = 0; x < objectWidth; x++) {
    //                 for (int y = 0; y < objectHeight; y++) {
    //                     gridPositionList.Add(originCellNum + new Vector2Int(x, y));
    //                 }
    //             }
    //             break;
    //         case Scriptable_UI_Item.Dir.Left:
    //         case Scriptable_UI_Item.Dir.Right:
    //             for (int x = 0; x < objectHeight; x++) {
    //                 for (int y = 0; y < objectWidth; y++) {
    //                     gridPositionList.Add(originCellNum + new Vector2Int(x, y));
    //                 }
    //             }
    //             break;
    //     }
    //     return gridPositionList;
    // }
    
    public void InsertItemToInventory(Vector2Int originCellNum, PlacedObject placedObject, Scriptable_UI_Item.Dir direction)
    {
        List<Vector2Int> cellNumList = placedObject.GetItemData().GetCellNumList(originCellNum, direction);
        foreach (Vector2Int cellNum in cellNumList)
        {
            grid.GetGridObject(cellNum.x, cellNum.y).SetGetPlacedObject = placedObject;
        }

        //placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.belongingCellNum = originCellNum;
        placedObject.belongingInventory = this;
        placedObject.rectTransform.SetParent(container);
        placedObject.direction = direction;

        //位置設定
        Vector2 placedObjectAnchoredPosition = grid.GetCellOriginAnchoredPosition(originCellNum.x, originCellNum.y); /*+ new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();*/
        // placedObjectTransform.rotation = Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        placedObject.rectTransform.anchoredPosition = placedObjectAnchoredPosition;

        //アイテムの比率をInventoryごとに変えられる
        placedObject.ImageSizeSet(cellSize * placedObject.GetItemData().width, cellSize * placedObject.GetItemData().height);

        //CheckCellObject();
    }

    public void RemoveItemFromInventory(Vector2Int originCellNum, PlacedObject placedObject, Scriptable_UI_Item.Dir direction)
    {
        List<Vector2Int> removeCellNumList = placedObject.GetItemData().GetCellNumList(originCellNum, direction);
        foreach (Vector2Int cellNum in removeCellNumList)
        {
            grid.GetGridObject(cellNum.x, cellNum.y).SetGetPlacedObject = null;
        }
        //placedObject.rectTransform.parent = null;
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
