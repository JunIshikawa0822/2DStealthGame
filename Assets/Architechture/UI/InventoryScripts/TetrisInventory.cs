using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TetrisInventory : A_Inventory
{
    public Grid<CellObject> grid;
    [SerializeField]
    private RectTransform container;
    [SerializeField]
    private RectTransform background;

    [SerializeField]
    private float _cellSize = 50;
    [SerializeField] 
    private int _gridWidth = 10;
    [SerializeField]
    private int _gridHeight = 10;

    public RectTransform GetContainer(){return container;}
    private IObjectPool _guiPool;
    //private ItemFacade _facade;
    private IStorage _openningStorage;

    void Awake()
    {
        grid = new Grid<CellObject>
        (
            _gridWidth,
            _gridHeight,
            _cellSize,
            (Grid<CellObject> grid, int cellPosition_x, int cellPosition_y) => new CellObject(cellPosition_x, cellPosition_y)
        );
    }

    public override void Init(IObjectPool objectPool)
    {
        _guiPool = objectPool;
    }

    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     for(int x = 0; x < 10; x++)
        //     {
        //         for(int y = 0; y < 10; y++)
        //         {
        //             BackGroundDebug(grid.GetCellObject(new CellNumber(x, y)));
        //         }
        //     }
        // }

        // if(Input.GetKeyDown(KeyCode.Q))
        // {
        //     for(int x = 0; x < 10; x++)
        //     {
        //         for(int y = 0; y < 10; y++)
        //         {
        //             BackGroundReset(grid.GetCellObject(new CellNumber(x, y)));
        //         }
        //     }
        // }
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

        if(cell.GUIInCell != null)
        {
            image.enabled = false;
        }
        else if(cell.Origin != null)
        {
            //Debug.Log("2");
            image.enabled = false;
        }
        else
        {
            //Debug.Log("3");
            image.enabled = true;
        }
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

    public override void OpenInventory(IStorage storage)
    {
        if(storage == null)return;

        _openningStorage = storage;
        
        foreach(IInventoryItem data in storage.GetItems())
        {
            LoadItem(data);
        }
    }

    public override void CloseInventory()
    {
        for(int x = 0; x < 10; x++)
        {
            for(int y = 0; y < 10; y++)
            {
                // Debug.Log(grid);
                grid.GetCellObject(new CellNumber(x, y)).Reset();
            }
        }

        int n = container.transform.childCount;

        for(int i = 0; i < container.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
    }

    private void LoadItem(IInventoryItem inventoryItem)
    {
        A_Item_GUI gui = _guiPool.GetFromPool() as A_Item_GUI;

        if(gui == null)return;

        gui.Init(inventoryItem);

        List<CellNumber> cellNumsList = gui.GetOccupyCellList(inventoryItem.Address, inventoryItem.Direction);

        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(cellNumsList[i]);

            if(cellNumsList[i] == inventoryItem.Address)
            {
                cellObject.Insert(gui);

                Vector3 newPosition = grid.GetCellOriginAnchoredPosition(inventoryItem.Address);

                gui.SetParent(container);
                gui.SetPivot(inventoryItem.Direction);
                gui.SetAnchorPosition(newPosition);
                gui.SetRotation(inventoryItem.Direction);
                gui.SetImageSize(_cellSize);
            }

            cellObject.Origin = inventoryItem.Address;
        }
    }

    public override void InsertItem(A_Item_GUI gui, CellNumber origin, IInventoryItem.ItemDir direction)
    {
        List<CellNumber> cellNumsList = gui.GetOccupyCellList(origin, direction);

        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(cellNumsList[i]);
            //originCellにStackしていく
            if(cellNumsList[i] == origin)
            {
                A_Item_GUI guiInCell = cellObject.GuiInCell;

                if(guiInCell == null)
                {
                    cellObject.Insert(gui);

                    Vector3 newPosition = grid.GetCellOriginAnchoredPosition(origin);

                    gui.Item.Address = origin;
                    gui.Item.Direction = direction;

                    gui.SetParent(container);
                    gui.SetPivot(direction);
                    gui.SetAnchorPosition(newPosition);
                    gui.SetRotation(direction);
                    gui.SetImageSize(_cellSize);

                    _openningStorage.Add(gui.Item);
                }
                else
                {
                    _openningStorage.Remove(guiInCell.Item);

                    cellObject.Stack(gui);

                    guiInCell = cellObject.GuiInCell;
                    _openningStorage.Add(guiInCell.Item);
                }
            }

            cellObject.Origin = origin;
        }
    }

    public override bool CanPlaceItem(A_Item_GUI gui, CellNumber originCellNum, IInventoryItem.ItemDir direction)
    {
        List<CellNumber> cellNumsList = gui.GetOccupyCellList(originCellNum, direction);

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

            CellObject cellObject = grid.GetCellObject(checkingCellNum);
            CellNumber cellOrigin = cellObject.Origin;

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
            if(originCell.IsStackable() == false)
            {
                Debug.Log("挿入先が満杯だよ!");
                canPlace = false;
            }

            if(!originCell.CheckEquality(gui.Item))
            {
                canPlace = false;
                Debug.Log("挿入先と入れたいアイテムの種類が根本的に違います");
            }
        }
        
        if(canPlace)
        {
            // Debug.Log("置けるよ！");
            return true;
        }
        else
        {
            // Debug.Log("置けないよ！");
            return false;
        }
    }

    public override void RemoveItem(CellNumber origin)
    {
        CellObject originCell = grid.GetCellObject(origin);
        A_Item_GUI gui = originCell.GuiInCell;

        IInventoryItem.ItemDir direction = gui.Item.Direction;
        List<CellNumber> removeCellNumList = gui.GetOccupyCellList(origin, direction);

        for(int i = 0; i < removeCellNumList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(removeCellNumList[i]);

            cellObject.Reset();
        }

        _openningStorage.Remove(gui.Item);
    }

    public override CellNumber ScreenPosToCellNum(Vector2 pos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, pos, null, out Vector2 convertPosition);
        return grid.GetCellNum(convertPosition);
    }

    public override bool IsValid(CellNumber cellNum)
    {
        return grid.IsValidCellNum(cellNum);
    }
}