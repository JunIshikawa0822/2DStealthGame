using System;
using System.Collections.Generic;
using TMPro;
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
        else if(cell.Origin != null)
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
        CellNumber cashedOriginCellNum = new CellNumber(0, 0);

        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellNumber checkingCellNum = cellNumsList[i];
            //CellNumber originCellNum;

#region 枠内かどうか確認
            bool isValidPosition = grid.IsValidCellNum(checkingCellNum);

            if (!isValidPosition)
            {
                Debug.Log("枠外だよ");
                canPlace = false;
                break;
            }
#endregion      
            //そもそも枠外であればcellObjectをとってこれないので、上記で枠外かどうか確認してからcellObjectを取得

            CellNumber originCellNum = grid.GetCellObject(checkingCellNum).Origin;
            Debug.Log(originCellNum);
            //originCellがnullの場合も含む

#region Stackすることを想定、確認したCellのOriginCellが全て同じか、下のセルに完全に重なっているか、はみ出しがないかを確認
            if(i == 0)
            {
                cashedOriginCellNum = originCellNum;
            }
            else
            {
                //originCellが同じでないものがある＝はみ出しがある
                if(cashedOriginCellNum != originCellNum)
                {
                    canPlace = false;
                    break;
                }

                cashedOriginCellNum = originCellNum;
            }
        }
#endregion

#region Stackすることを想定、同じItemを入れているかどうか
        if(grid.GetCellObject(cashedOriginCellNum).CheckItem(item) == false)
        {
            canPlace = false;
        }
#endregion

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
    public uint InsertItemToInventory(Item_GUI item, CellNumber originCellNum, Item_GUI.ItemDir direction)
    {
        List<CellNumber> cellNumsList = item.GetCellNumList(direction, originCellNum);

        uint remain = 0;
        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(cellNumsList[i]);

            //originCellにStackしていく
            if(cellNumsList[i] == originCellNum)
            {
                remain = cellObject.InsertItem(item, item.StackingNum);
            }
            //cellのOriginNumberをセット
            cellObject.InsertOriginCellNumber(originCellNum);
        }

        item.SetBelongings(this, originCellNum, direction, inventoryRectTransform);
        return remain;
    }

#endregion

    public void RemoveItemFromInventory(CellNumber originCellNum, Item_GUI item, Item_GUI.ItemDir direction)
    {
        List<CellNumber> removeCellNumList = item.GetCellNumList(direction, originCellNum);
        
        for(int i = 0; i < removeCellNumList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(removeCellNumList[i]);

            cellObject.ResetCell();
        }

        foreach(CellObject cellNum in grid.gridArray)
        {
            BackGroundDebug(cellNum);
        }
    }
}