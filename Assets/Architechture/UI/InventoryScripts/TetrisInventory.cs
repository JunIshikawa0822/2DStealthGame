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
    [HideInInspector]
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
            (Grid<CellObject> grid, int cellPosition_x, int cellPosition_y) => new CellObject(cellPosition_x, cellPosition_y)
        );

        BackGroundSetUp();
    }

    void Start()
    {
    
    }

    void Update()
    {
    
    }

    void BackGroundSetUp()
    {
        background.sizeDelta = new Vector2(gridWidth, gridHeight) * cellSize;
        background.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
    }

    void BackGroundDebug(CellObject cell)
    {
        int num = 0;
        num = cell.position_y * 10 + cell.position_x;

        if(cell.GetPlacedObject() != null)
        {
            background.GetChild(num).GetComponent<Image>().enabled = false;
        }
        else
        {
            background.GetChild(num).GetComponent<Image>().enabled = true;
        }
    }

    public bool CanPlaceItem(PlacedObject placedObject, Vector2Int originCellNum, Scriptable_UI_Item.ItemDir direction)
    {   
        //オブジェクトが占有するマス目を計算
        List<Vector2Int> gridPositionList = placedObject.GetItemData().GetCellNumList(direction, originCellNum);

        bool canPlace = true;
        int instanceID = 0;

        for(int i = 0; i < gridPositionList.Count; i++)
        {
            Vector2Int gridPosition = gridPositionList[i];
            CellObject cellObject = grid.GetGridObject(gridPosition.x, gridPosition.y);

#region AreaCheck
            bool isValidPosition = grid.IsValidCellNum(gridPosition);
            if (!isValidPosition)
            {
                Debug.Log("枠外だよ");
                canPlace = false;
                break;
            }
#endregion
#region IdentityCheck
            //すべてのCellが同一のinstanceIDで埋まっているか確認
            int cellobjectID = cellObject.GetPlacedObject() == null ? 0 : cellObject.GetPlacedObject().GetInstanceID();
            if(i != 0)
            {
                if(instanceID != cellobjectID)
                {
                    Debug.Log("同一ではないよ");
                    canPlace = false;
                    break;
                }
                instanceID = cellobjectID;
            }
            else
            {
                instanceID = cellobjectID;
            }         
#endregion
#region TypeAndStackableCheck
            //すべてのCellが挿入したいPlacedObjectと同じタイプかつ空きがあるか確認
            bool canInsert = cellObject.CanInsertToCellObject(placedObject);
            if(!canInsert)
            {
                Debug.Log("空きがない/別タイプ");
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
    
    public void InsertItemToInventory(Vector2Int originCellNum, PlacedObject placedObject, int insertObjectNum, Scriptable_UI_Item.ItemDir direction, out int remainNum)
    {
        remainNum = 0;
        if(placedObject == null)return;

        List<Vector2Int> cellNumList = placedObject.GetItemData().GetCellNumList(direction, originCellNum);
        // Debug.Log(string.Join(",", cellNumList));

        PlacedObject cashedPlacedObject = null;
        //int stackNumInCell = 0;

        Debug.Log("いれたい数 : " + insertObjectNum);

        foreach (Vector2Int cellNum in cellNumList)
        {
            CellObject cellObject =  grid.GetGridObject(cellNum.x, cellNum.y);

            //セルに入っているオブジェクトをキャッシュ
            cashedPlacedObject = cellObject.GetPlacedObject();
            //新しいのを入れる
            cellObject.InsertToCellObject(placedObject);

            Debug.Log("cellNum : " + cellNum);

            for(int i = insertObjectNum; i >= 1; i--)
            // for(int i = 1; i <= insertObjectNum; i++)
            {
                bool stackable = cellObject.GetStackability();
                if(stackable)
                {
                    Debug.Log("残り" + i + "個");
                    cellObject.SetStackNum();
                    cellObject.SetStackability();
                }
                else
                {
                    //remainNum = insertObjectNum - i;
                    remainNum = i;
                    Debug.Log(i + "個は入らない remainNum : " + remainNum);
                    break;
                }
            }

            cellObject.SetPlacedObjectStackNum();
            //stackNumInCell = cellObject.GetStackNum();
        }

        placedObject.SetBelonging(this, originCellNum, direction, container);
        Vector2Int rotationAnchorCellNumOffset = placedObject.GetItemData().GetRotationOffset(direction);
        // Debug.Log(rotationAnchorCellNumOffset);

        Vector2 placedObjectAnchoredPosition = grid.GetCellOriginAnchoredPosition(originCellNum.x, originCellNum.y) + new Vector2(rotationAnchorCellNumOffset.x, rotationAnchorCellNumOffset.y) * cellSize;
        test1.anchoredPosition = placedObjectAnchoredPosition;

        placedObject.GetRectTransform().anchoredPosition = placedObjectAnchoredPosition;
        placedObject.GetRectTransform().rotation = Quaternion.Euler(0, 0, placedObject.GetItemData().GetRotationAngle(direction));
        placedObject.ImageSizeSet(cellSize);

        //重ねている
        if(cashedPlacedObject != null)
        {
            Debug.Log("バグの原因か？");
            //placedObject.StackNumInit(stackNumInCell);
            cashedPlacedObject.OnDestroy();
        }

        foreach(CellObject cellNum in grid.gridArray)
        {
            BackGroundDebug(cellNum);
        }
    }

    public void RemoveItemFromInventory(Vector2Int originCellNum, PlacedObject placedObject, Scriptable_UI_Item.ItemDir direction)
    {
        List<Vector2Int> removeCellNumList = placedObject.GetItemData().GetCellNumList(direction, originCellNum);
        // Debug.Log("remove : " + string.Join(",", removeCellNumList));
        foreach (Vector2Int cellNum in removeCellNumList)
        {
            CellObject cellObject = grid.GetGridObject(cellNum.x, cellNum.y);
            cellObject.InsertToCellObject(null);
            cellObject.SetStackNum();
        }

        foreach(CellObject cellNum in grid.gridArray)
        {
            BackGroundDebug(cellNum);
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