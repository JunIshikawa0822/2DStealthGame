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

    public RectTransform test1;

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

    public bool TryPlaceItem(PlacedObject placedObject, Vector2Int originCellNum, Scriptable_UI_Item.ItemDir direction)
    {   
        //オブジェクトが占有するマス目を計算
        List<Vector2Int> gridPositionList = placedObject.GetItemData().GetCellNumList(direction, originCellNum);
        Debug.Log(string.Join(",", gridPositionList));

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
#region editing
            //CellがNullの場合
            //canPlace = true;

            //Cellがすでに埋まっており、そのCellすべてが引数のplacedObjectと同じタイプかつ、Stack可能なら
            //canPlace = true;

            //Cellがすでに埋まっており、Stackができないなら
            //canPlace = false;

            //Cellがすでに埋まっている場合
            if(grid.GetGridObject(gridPosition.x, gridPosition.y).GetPlacedObject() != null)
            {
                canPlace = false;
                break;
            }
        }
#endregion

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
    
    public void InsertItemToInventory(Vector2Int originCellNum, PlacedObject placedObject, Scriptable_UI_Item.ItemDir direction, out Vector2 SetPosition)
    {
        List<Vector2Int> cellNumList = placedObject.GetItemData().GetCellNumList(direction, originCellNum);
        foreach (Vector2Int cellNum in cellNumList)
        {
            grid.GetGridObject(cellNum.x, cellNum.y).SetPlacedObject(placedObject);
        }

        placedObject.BelongingsInit(this, originCellNum, direction, container);

        //位置補正
        Vector2Int rotationAnchorCellNumOffset = placedObject.GetItemData().GetRotationOffset(direction);
        Debug.Log(rotationAnchorCellNumOffset);

        //位置設定
        Vector2 placedObjectAnchoredPosition = grid.GetCellOriginAnchoredPosition(originCellNum.x, originCellNum.y) + new Vector2(rotationAnchorCellNumOffset.x, rotationAnchorCellNumOffset.y) * cellSize;
        placedObject.GetRectTransform().anchoredPosition = placedObjectAnchoredPosition;
        SetPosition = placedObjectAnchoredPosition;

        placedObject.GetRectTransform().rotation = Quaternion.Euler(0, 0, placedObject.GetItemData().GetRotationAngle(direction));
        
        //アイテムの比率をInventoryごとに変えられる
        placedObject.ImageSizeSet(cellSize);

    }

    public void RemoveItemFromInventory(Vector2Int originCellNum, PlacedObject placedObject, Scriptable_UI_Item.ItemDir direction)
    {
        List<Vector2Int> removeCellNumList = placedObject.GetItemData().GetCellNumList(direction, originCellNum);
        //int i = 0;
        foreach (Vector2Int cellNum in removeCellNumList)
        {
            //i++;
            grid.GetGridObject(cellNum.x, cellNum.y).SetPlacedObject(null);
            // Debug.Log(i);
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
                if(grid.GetGridObject(i, j).GetPlacedObject() == null) continue;
                testlist.Add(new Vector2Int(i, j));
                //Debug.Log("(" + i + "," + j + ") : " + grid.GetGridObject(i, j).SetGetPlacedObject);     
            }
        }

        Debug.Log(string.Join("," , testlist));
        Debug.Log("チェック終了");
    }
}
