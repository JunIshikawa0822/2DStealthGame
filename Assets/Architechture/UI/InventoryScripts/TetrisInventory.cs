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

    public RectTransform GetContainer(){return container;}

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
        num = (9 - cell.position_y) * 10 + cell.position_x;

        if(cell.GetItemInCell() != null)
        {
            background.GetChild(num).GetComponent<Image>().enabled = false;
        }
        else
        {
            background.GetChild(num).GetComponent<Image>().enabled = true;
        }
    }

    public bool CanPlaceItem(Item_GUI item, Vector2Int originCellNum, Item_GUI.ItemDir direction)
    {   
        //オブジェクトが占有するマス目を計算
        List<Vector2Int> gridPositionList = item.GetCellNumList(direction, originCellNum);

        bool canPlace = true;
        //int instanceID = 0;

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

#region すでにそのCellにオブジェクトが入っているか確認
            if(cellObject.GetItemInCell() != null)
            {
                canPlace = false;
                break;
            }
#endregion
#region IdentityCheck
            // //すべてのCellが同一のinstanceIDで埋まっているか確認
            // int cellobjectID = cellObject.GetPlacedObject() == null ? 0 : cellObject.GetPlacedObject().GetInstanceID();
            // if(i != 0)
            // {
            //     if(instanceID != cellobjectID)
            //     {
            //         Debug.Log("同一ではないよ");
            //         canPlace = false;
            //         break;
            //     }
            //     instanceID = cellobjectID;
            // }
            // else
            // {
            //     instanceID = cellobjectID;
            // }         
#endregion
#region TypeAndStackableCheck
            // //すべてのCellが挿入したいPlacedObjectと同じタイプかつ空きがあるか確認
            // bool canInsert = cellObject.CanInsertToCellObject(item);
            // if(!canInsert)
            // {
            //     Debug.Log("空きがない/別タイプ");
            //     canPlace = false;
            //     break;
            // }
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
    
    public void InsertItemToInventory(Item_GUI item, Vector2Int originCellNum, Item_GUI.ItemDir direction/*, out int remainNum*/)
    {
        //remainNum = 0;
        if(item == null)return;

        List<Vector2Int> cellNumList = item.GetCellNumList(direction, originCellNum);
        // Debug.Log(string.Join(",", cellNumList));

        Item_GUI cashedItem = null;
        //int stackNumInCell = 0;

        foreach (Vector2Int cellNum in cellNumList)
        {
            CellObject cellObject =  grid.GetGridObject(cellNum.x, cellNum.y);

            #region なんで？
            //セルに入っているオブジェクトをキャッシュ
            cashedItem = cellObject.GetItemInCell();
            #endregion

            //新しいのを入れる
            cellObject.InsertToCellObject(item);
            //入れたい数
            // int stackNum = insertObjectNum/*placedObject.GetStackNum()*/;

            // for(int i = 1; i <= stackNum; i++)
            // {
            //     bool stackable = cellObject.GetStackability();
            //     //insertNum = i;
                
            //     if(stackable)
            //     {
            //         cellObject.SetStackNum();
            //         cellObject.SetStackability();
            //     }
            //     else
            //     {
            //         remainNum = stackNum - i;
            //         break;
            //     }
            // } 
            // stackNumInCell = cellObject.GetStackNum();
        }

        item.SetBelonging(this, originCellNum, direction, container);
        Vector2Int rotationAnchorCellNumOffset = item.GetRotationOffset(direction);
        // Debug.Log(rotationAnchorCellNumOffset);

        Vector2 placedObjectAnchoredPosition = grid.GetCellOriginAnchoredPosition(originCellNum.x, originCellNum.y) + new Vector2(rotationAnchorCellNumOffset.x, rotationAnchorCellNumOffset.y) * cellSize;
        test1.anchoredPosition = placedObjectAnchoredPosition;

        item.SetAnchorPosition(placedObjectAnchoredPosition);
        item.SetRotation(Quaternion.Euler(0, 0, item.GetRotationAngle(direction)));
        //item.ImageSizeSet(cellSize);

        // //重ねている
        // if(cashedPlacedObject != null)
        // {
        //     placedObject.StackNumInit(stackNumInCell);
        //     cashedPlacedObject.OnDestroy();
        // }

        foreach(CellObject cellNum in grid.gridArray)
        {
            BackGroundDebug(cellNum);
        }
    }

    public void RemoveItemFromInventory(Vector2Int originCellNum, Item_GUI item, Item_GUI.ItemDir direction)
    {
        List<Vector2Int> removeCellNumList = item.GetCellNumList(direction, originCellNum);
        // Debug.Log("remove : " + string.Join(",", removeCellNumList));
        foreach (Vector2Int cellNum in removeCellNumList)
        {
            CellObject cellObject = grid.GetGridObject(cellNum.x, cellNum.y);
            cellObject.InsertToCellObject(null);
            //cellObject.SetStackNum();
        }

        foreach(CellObject cellNum in grid.gridArray)
        {
            BackGroundDebug(cellNum);
        }
    }

    // public void CheckCellObject()
    // {
    //     List<Vector2Int> testlist = new List<Vector2Int>();

    //     Debug.Log("チェック開始");
    //     for(int i = 0; i < gridWidth; i++)
    //     {
    //         for(int j = 0; j < gridHeight; j++)
    //         {
    //             if(grid.GetGridObject(i, j).GetPlacedObject() == null) continue;
    //             testlist.Add(new Vector2Int(i, j));  
    //         }
    //     }

    //     Debug.Log(string.Join("," , testlist));
    //     Debug.Log("チェック終了");
    // }
}