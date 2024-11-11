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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            for(int x = 0; x < 10; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    BackGroundDebug(grid.GetCellObject(new CellNumber(x, y)));
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            for(int x = 0; x < 10; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    BackGroundReset(grid.GetCellObject(new CellNumber(x, y)));
                }
            }
        }
    }

    void BackGroundSetUp()
    {
        background.sizeDelta = new Vector2(_gridWidth, _gridHeight) * _cellSize;
        background.GetComponent<GridLayoutGroup>().cellSize = new Vector2(_cellSize, _cellSize);
    }

    void BackGroundReset(CellObject cell)
    {
        int num = 0;
        num = (9 - cell.position_y) * 10 + cell.position_x;
        Image image = background.GetChild(num).GetComponent<Image>();

        // string cellNum = cell.Origin == null ? "null" : cell.Origin.ToString();
        // string item = cell.GetItemInCell() == null ? "null" : "item";
        // Debug.Log($"({cell.position_x},{cell.position_y})のOriginは{cellNum}です!!!");
        // Debug.Log($"({cell.position_x},{cell.position_y})には{item}が入っています!!!");

        image.enabled = true;
    }

    void BackGroundDebug(CellObject cell)
    {
        int num = 0;
        num = (9 - cell.position_y) * 10 + cell.position_x;
        Image image = background.GetChild(num).GetComponent<Image>();

        // string cellNum = cell.Origin == null ? "null" : cell.Origin.ToString();
        // string item = cell.GetItemInCell() == null ? "null" : "item";
        // Debug.Log($"({cell.position_x},{cell.position_y})のOriginは{cellNum}です!!!");
        // Debug.Log($"({cell.position_x},{cell.position_y})には{item}が入っています!!!");

        if(cell.GetItemInCell() != null)
        {
            image.enabled = false;
        }
        else if(cell.Origin != null)
        {
            Debug.Log("2");
            image.enabled = false;
        }
        else
        {
            Debug.Log("3");
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

            Debug.Log("枠外ではない");
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
                    Debug.Log("はみ出てるよ");
                    break;
                }

                cashedOriginCellNum = originCellNum;
            }
        }
#endregion
        Debug.Log("はみ出てないよ");
        Debug.Log(cashedOriginCellNum);
#region Stackすることを想定、そのCellにStackできるかどうか

        CellObject originCell = grid.GetCellObject(cashedOriginCellNum);
        if(cashedOriginCellNum != null)
        {
            if(!originCell.CheckEquality(item))
            {
                canPlace = false;
                Debug.Log("挿入先と入れたいアイテムの種類が根本的に違います");
            }

            if(originCell.GetStackabilty() == false)
            {
                Debug.Log("挿入先が、満杯だよ!");
                canPlace = false;
            }
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
        Debug.Log("以下にデータを入れる");
        Debug.Log(string.Join(", ", cellNumsList));

        uint remain = 0;
        for(int i = 0; i < cellNumsList.Count; i++)
        {
            //Debug.Log(v);
            CellObject cellObject =  grid.GetCellObject(cellNumsList[i]);
            //originCellにStackしていく
            if(cellNumsList[i] == originCellNum)
            {
                remain = cellObject.InsertItem(item, item.StackingNum);
                cellObject.SetStack();
            }
            //cellのOriginNumberをセット
            cellObject.Origin = originCellNum;

            //Debug.Log($"{cellNumsList[i]}のoriginは{cellObject.Origin}です");
            //Debug.Log($"{cellNumsList[i]}には{cellObject.GetItemInCell()}が入っています");
        }

        

        item.SetBelongings(this, originCellNum, direction, inventoryRectTransform);
        item.SetAnchor(direction);
        item.SetAnchorPosition(grid.GetCellOriginAnchoredPosition(originCellNum.x, originCellNum.y));
        item.SetRotation(Quaternion.Euler(0, 0, item.GetRotationAngle(direction)));

        // foreach(CellObject cellNum in grid.gridArray)
        // {
        //     BackGroundDebug(cellNum);
        // }

        return remain;
    }

#endregion

    public void RemoveItemFromInventory(CellNumber originCellNum, Item_GUI item, Item_GUI.ItemDir direction)
    {
        List<CellNumber> removeCellNumList = item.GetCellNumList(direction, originCellNum);
        Debug.Log("以下からデータRemove");
        Debug.Log(string.Join(", ", removeCellNumList));
        
        for(int i = 0; i < removeCellNumList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(removeCellNumList[i]);

            cellObject.ResetCell();
        }
    }
}