using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TetrisInventory : MonoBehaviour, IInventory
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

        if(cell.ItemInCell != null)
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


    public uint InsertItemToInventory(Item_GUI item, CellNumber originCellNum, Item_GUI.ItemDir direction)
    {
        List<CellNumber> cellNumsList = item.GetCellNumList(originCellNum, direction);
        Debug.Log("以下にデータを入れる");
        Debug.Log(string.Join(", ", cellNumsList));

        uint remain = 0;
        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(cellNumsList[i]);
            //originCellにStackしていく
            if(cellNumsList[i] == originCellNum)
            {
                remain = cellObject.InsertItem(item, item.StackingNum);
                cellObject.SetStack();
            }
            cellObject.Origin = originCellNum;
        }

        Debug.Log($"オリジン:{originCellNum}");
        Vector3 newPosition = grid.GetCellOriginAnchoredPosition(originCellNum);

        item.SetBelongings(this, newPosition, direction);
        item.RectTransform.SetParent(container);
        item.SetPivot(direction);
        item.SetAnchorPosition(newPosition);
        item.SetRotation(direction);
        item.SetImageSize(_cellSize);

        return remain;
    }

    public CellNumber ScreenPosToCellNum(Vector2 pos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, pos, null, out Vector2 convertPosition);
        return grid.GetCellNum(convertPosition);
    }

    public void DecreaseItemNum(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction, uint num)
    {
        Vector3 coordinateOffset = new Vector3(_cellSize/2, -_cellSize/2);
        CellNumber originCellNum = ScreenPosToCellNum(originPos + coordinateOffset);
        List<CellNumber> cellNumsList = item.GetCellNumList(originCellNum, direction);

        Debug.Log("なんばん:" + cellNumsList[0]);
        CellObject cellObject =  grid.GetCellObject(cellNumsList[0]);
        uint remain = grid.GetCellObject(cellNumsList[0]).DecreaseItem(num);
        cellObject.SetStack();

        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellObject cell =  grid.GetCellObject(cellNumsList[i]);

            if(remain <= 0)
            {
                cell.ResetCell();
            }
        }
    }

    public uint InsertItemToInventory(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {
        Vector3 coordinateOffset = new Vector3(_cellSize/2, -_cellSize/2);
        CellNumber originCellNum = ScreenPosToCellNum(originPos + coordinateOffset);
        List<CellNumber> cellNumsList = item.GetCellNumList(originCellNum, direction);

        // Debug.Log($"origin : {originCellNum}");
        // Debug.Log("以下にデータを入れる");
        // Debug.Log(string.Join(", ", cellNumsList));

        uint remain = 0;
        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(cellNumsList[i]);
            //originCellにStackしていく
            if(cellNumsList[i] == originCellNum)
            {
                remain = cellObject.InsertItem(item, item.StackingNum);
                cellObject.SetStack();
            }
            cellObject.Origin = originCellNum;
        }

        Debug.Log($"オリジン:{originCellNum}");
        Vector3 newPosition = grid.GetCellOriginAnchoredPosition(originCellNum);

        item.SetBelongings(this, newPosition, direction);
        item.RectTransform.SetParent(container);
        item.SetPivot(direction);
        item.SetAnchorPosition(newPosition);
        item.SetRotation(direction);
        item.SetImageSize(_cellSize);

        return remain;
    }

    public void RemoveItemFromInventory(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {
        //OriginCellの左上の座標をCellNumに変換すると、正しく変換されない可能性がある
        //OriginCellの中心に補正をしてからCellNumに変換しよう
        Vector3 coordinateOffset = new Vector3(_cellSize/2, -_cellSize/2);
        CellNumber originCellNum = ScreenPosToCellNum(originPos + coordinateOffset);
        List<CellNumber> removeCellNumList = item.GetCellNumList(originCellNum, direction);

        Debug.Log(originCellNum);
        Debug.Log("以下からデータRemove");
        Debug.Log(string.Join(", ", removeCellNumList));
        
        for(int i = 0; i < removeCellNumList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(removeCellNumList[i]);

            cellObject.ResetCell();
        }
    }

    public void RemoveItemFromInventory(CellNumber originCellNum)
    {
        Item_GUI item = grid.GetCellObject(originCellNum).ItemInCell;
        Item_GUI.ItemDir direction = item.ItemDirection;
        List<CellNumber> removeCellNumList = item.GetCellNumList(originCellNum, direction);

        for(int i = 0; i < removeCellNumList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(removeCellNumList[i]);

            cellObject.ResetCell();
        }
    }

    public bool CanPlaceItem(Item_GUI item, CellNumber originCellNum, Item_GUI.ItemDir direction)
    {
        List<CellNumber> cellNumsList = item.GetCellNumList(originCellNum, direction);

        bool canPlace = true;
        CellNumber cashedOriginCellNum = new CellNumber(0, 0);

        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellNumber checkingCellNum = cellNumsList[i];
            bool isValidPosition = grid.IsValidCellNum(checkingCellNum);

            if (!isValidPosition)
            {
                canPlace = false;
                break;
            }
            //そもそも枠外であればcellObjectをとってこれないので、上記で枠外かどうか確認してからcellObjectを取得
            CellNumber cellOrigin = grid.GetCellObject(checkingCellNum).Origin;

            if(i == 0)
            {
                cashedOriginCellNum = cellOrigin;
            }
            else
            {
                //originCellが同じでないものがある＝はみ出しがある
                if(cashedOriginCellNum != cellOrigin)
                {
                    canPlace = false;
                    break;
                }

                cashedOriginCellNum = cellOrigin;
            }
        }

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
                Debug.Log("挿入先が満杯だよ!");
                canPlace = false;
            }
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


    public bool CanPlaceItem(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {
        Vector3 coordinateOffset = new Vector3(_cellSize/2, -_cellSize/2);
        CellNumber originCellNum = ScreenPosToCellNum(originPos + coordinateOffset);
        List<CellNumber> cellNumsList = item.GetCellNumList(originCellNum, direction);

        bool canPlace = true;
        CellNumber cashedOriginCellNum = new CellNumber(0, 0);

        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellNumber checkingCellNum = cellNumsList[i];
            bool isValidPosition = grid.IsValidCellNum(checkingCellNum);

            if (!isValidPosition)
            {
                canPlace = false;
                break;
            }
            //そもそも枠外であればcellObjectをとってこれないので、上記で枠外かどうか確認してからcellObjectを取得
            CellNumber cellOrigin = grid.GetCellObject(checkingCellNum).Origin;

            if(i == 0)
            {
                cashedOriginCellNum = cellOrigin;
            }
            else
            {
                //originCellが同じでないものがある＝はみ出しがある
                if(cashedOriginCellNum != cellOrigin)
                {
                    canPlace = false;
                    break;
                }

                cashedOriginCellNum = cellOrigin;
            }
        }

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
                Debug.Log("挿入先が満杯だよ!");
                canPlace = false;
            }
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

    public bool IsValid(Vector3 originPos)
    {
        Vector3 coordinateOffset = new Vector3(_cellSize/2, -_cellSize/2);
        CellNumber originCellNum = ScreenPosToCellNum(originPos + coordinateOffset);

        return grid.IsValidCellNum(originCellNum);
    }
}