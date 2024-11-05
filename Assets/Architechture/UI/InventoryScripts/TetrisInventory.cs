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
    private float _cellSize = 50;
    [SerializeField] 
    private int _gridWidth = 10;
    [SerializeField]
    private int _gridHeight = 10;

    public RectTransform GetContainer(){return container;}

    void Awake()
    {
        inventoryRectTransform = this.GetComponent<RectTransform>();

        grid = new Grid<CellObject>
        (
            _gridWidth,
            _gridHeight,
            _cellSize,
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
        background.sizeDelta = new Vector2(_gridWidth, _gridHeight) * _cellSize;
        background.GetComponent<GridLayoutGroup>().cellSize = new Vector2(_cellSize, _cellSize);
    }

    void BackGroundDebug(CellObject cell)
    {
        int num = 0;
        num = (9 - cell.position_y) * 10 + cell.position_x;
        Image image = background.GetChild(num).GetComponent<Image>();

        if(cell.GetItemInCell() != null)
        {
            image.enabled = false;
        }
        else if(cell.GetOriginCellNum() != null)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
        }
    }

    public bool CanPlaceItem(Item_GUI item, List<CellNumber> cellNumsList)
    {
        bool canPlace = true;
        CellObject cellObject;
        CellNumber cellsOriginCellNum = new CellNumber(0, 0);

        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellNumber cellNum = cellNumsList[i];
            //CellNumber originCellNum;

#region 枠内かどうか確認
            bool isValidPosition = grid.IsValidCellNum(cellNum);

            if (!isValidPosition)
            {
                Debug.Log("枠外だよ");
                canPlace = false;
                break;
            }
#endregion

#region そのセルのoriginCellにオブジェクトが入っているかどうか確認
            
            //そもそも枠外であればcellObjectをとってこれないので、枠外かどうか確認してからcellObjectを取得
            cellObject = grid.GetCellObject(cellNum);
            //cellObjectのoriginのcellを探す
            CellNumber originCellNum = cellObject.GetOriginCellNum();

            Debug.Log(originCellNum);
            //originCellNumがnullでないとき
            if(i == 0)
            {
                // //originCellがStack可能かどうか
                // if(!grid.GetCellObject(originCellNum).CanStack(item.GetItemData()))
                // {
                //     Debug.Log("もう入れられない");
                //     canPlace = false;
                //     break;
                // }
            }
#endregion

        }

        if(canPlace)
        {
            Debug.Log("置けるよ！");
            return true;
        }
        else
        {
            Debug.Log("置けないよ！");
            return false;
        }
    }

#region 変更がかなり必要 フローチャートを書こう
    public void InsertItemToInventory(Item_GUI item, CellNumber originCellNum, Item_GUI.ItemDir direction/*, out int remainNum*/)
    {
        //remainNum = 0;
        if(item == null)return;
        List<CellNumber> cellNumsList = item.GetCellNumList(direction, originCellNum);
        //Debug.Log(string.Join(", ", cellNumsList));
        //Debug.Log(cellNumsList.Count);
        // Vector2Int[] cells = item.GetOccupyCells(direction, originCellNum);
        // Debug.Log(string.Join(", ", cells));
        
        if(!CanPlaceItem(item, cellNumsList))return;

        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(cellNumsList[i]);

            if(cellNumsList[i] == originCellNum)
            {
                Scriptable_ItemData itemData = item.GetItemData();

                //入ってるぶん、もう片方に移せるかどうか試す
                for(int l = 0;  l < item.GetStackNum(); l++)
                {
                    if(cellObject.CanStack(itemData))
                    {
                        cellObject.InsertItem(item);
                        Debug.Log("Insert");
                    }
                    else
                    {
                        Debug.Log("これ以上はいらない");
                        break;
                    }
                }
            }
            cellObject.InsertOriginCellNumber(originCellNum);
        }

        //もうすべて移し終えた場合
        if(item.GetStackNum() < 1)
        {
            //grid.GetCellObject(originCellNum).GetItemInCell().SetStack();
            Destroy(this.gameObject);
        }
        else
        {
            //この
            item.SetBelongings(this, originCellNum, direction, container);
            item.SetAnchor(direction);
            item.SetAnchorPosition(grid.GetCellOriginAnchoredPosition(originCellNum.x, originCellNum.y));
            item.SetRotation(Quaternion.Euler(0, 0, item.GetRotationAngle(direction)));
            item.SetStack(grid.GetCellObject(originCellNum).GetStackNum());

            foreach(CellObject cellNum in grid.gridArray)
            {
                BackGroundDebug(cellNum);
            }
        }

        
    }

#endregion

    public void RemoveItemFromInventory(CellNumber originCellNum, Item_GUI item, Item_GUI.ItemDir direction)
    {
        List<CellNumber> removeCellNumList = item.GetCellNumList(direction, originCellNum);
        
        for(int i = 0; i < removeCellNumList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(removeCellNumList[i]);

            if(removeCellNumList[i] == originCellNum)cellObject.InsertItem(null);
            cellObject.InsertOriginCellNumber(null);
        }

        foreach(CellObject cellNum in grid.gridArray)
        {
            BackGroundDebug(cellNum);
        }
    }
}