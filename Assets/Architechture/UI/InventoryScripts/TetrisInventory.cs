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

    public bool CanPlaceItem(PlacedObject placedObject, Vector2Int originCellNum, Scriptable_UI_Item.ItemDir direction)
    {   
        //オブジェクトが占有するマス目を計算
        List<Vector2Int> gridPositionList = placedObject.GetItemData().GetCellNumList(direction, originCellNum);
        // Debug.Log(string.Join(",", gridPositionList));

        bool canPlace = true;
        int instanceID = 0;

        foreach (Vector2Int gridPosition in gridPositionList)
        {
            bool isValidPosition = grid.IsValidCellNum(gridPosition);
            CellObject cellObject = grid.GetGridObject(gridPosition.x, gridPosition.y);

#region AreaCheck
            if (!isValidPosition)
            {
                // Not valid
                canPlace = false;
                break;
            }
#endregion
#region IdentityCheck
            //すべてのCellが同一のinstanceIDで埋まっているか確認
            int cellobjectID = cellObject.GetPlacedObject() == null ? 0 : cellObject.GetPlacedObject().GetInstanceID();

            if(instanceID != cellobjectID)
            {
                canPlace = false;
                break;
            }
            instanceID = cellobjectID;
#endregion
#region TypeAndStackableCheck
            //すべてのCellが挿入したいPlacedObjectと同じタイプかつ空きがあるか確認
            bool canInsert = cellObject.CanInsertToCellObject(placedObject);
            if(!canInsert)
            {
                canPlace = false;
                break;
            }
#endregion
        }

        if (canPlace)
        {
            //Debug.Log("CanPlaceItem : true");
            return true;
        } 
        else 
        {
            //Debug.Log("CanPlaceItem : false");
            return false;
        }
    }
    
    public void InsertItemToInventory(Vector2Int originCellNum, PlacedObject placedObject, Scriptable_UI_Item.ItemDir direction, out int remainNum)
    {
        remainNum = 0;
        if(placedObject == null)return;

        List<Vector2Int> cellNumList = placedObject.GetItemData().GetCellNumList(direction, originCellNum);
        
        PlacedObject cashedPlacedObject = null;
        int stackNumInCell = 0;

        foreach (Vector2Int cellNum in cellNumList)
        {
            CellObject cellObject =  grid.GetGridObject(cellNum.x, cellNum.y);

            //セルに入っているオブジェクトをキャッシュ
            cashedPlacedObject = cellObject.GetPlacedObject();
            //新しいのを入れる
            cellObject.InsertToCellObject(placedObject);
            //入れたい数
            int stackNum = placedObject.GetStackNum();

            for(int i = 1; i <= stackNum; i++)
            {
                bool stackable = cellObject.GetStackability();
                //insertNum = i;
                
                if(stackable)
                {
                    cellObject.SetStackNum();
                    cellObject.SetStackability();
                }
                else
                {
                    remainNum = stackNum - i;
                    break;
                }
            } 
            stackNumInCell = cellObject.GetStackNum();
        }

        placedObject.SetBelonging(this, originCellNum, direction, container);
        Vector2Int rotationAnchorCellNumOffset = placedObject.GetItemData().GetRotationOffset(direction);
        Vector2 placedObjectAnchoredPosition = grid.GetCellOriginAnchoredPosition(originCellNum.x, originCellNum.y) + new Vector2(rotationAnchorCellNumOffset.x, rotationAnchorCellNumOffset.y) * cellSize;
        placedObject.GetRectTransform().anchoredPosition = placedObjectAnchoredPosition;
        placedObject.GetRectTransform().rotation = Quaternion.Euler(0, 0, placedObject.GetItemData().GetRotationAngle(direction));
        placedObject.ImageSizeSet(cellSize);

        if(cashedPlacedObject != null)
        {
            placedObject.StackNumInit(stackNumInCell);
            cashedPlacedObject.OnDestroy();
        }
    }

    public void RemoveItemFromInventory(Vector2Int originCellNum, PlacedObject placedObject, Scriptable_UI_Item.ItemDir direction)
    {
        List<Vector2Int> removeCellNumList = placedObject.GetItemData().GetCellNumList(direction, originCellNum);

        foreach (Vector2Int cellNum in removeCellNumList)
        {
            CellObject cellObject = grid.GetGridObject(cellNum.x, cellNum.y);
            cellObject.InsertToCellObject(null);
            cellObject.SetStackNum();
        }
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
            }
        }

        Debug.Log(string.Join("," , testlist));
        Debug.Log("チェック終了");
    }
}