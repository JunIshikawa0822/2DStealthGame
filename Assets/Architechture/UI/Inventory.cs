using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
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

    //public GUI_Item GUI_Item_Prefab;
    private IObjectPool _objectPool;

    private Storage _openningStorage;

    public event Func<ItemData, Transform, GUI_Item> itemInstantiateEvent; 

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

    void OnSetUp(IObjectPool guiPool)
    {
        _objectPool = guiPool;
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

    public void OpenInventory(Storage storage)
    {
        _openningStorage = storage;
        //Debug.Log(itemInstantiateEvent);
        foreach(ItemData data in storage.ItemList)
        {
            LoadItem(data);
        }
    }

    public void CloseInventory()
    {
        for(int x = 0; x < 10; x++)
        {
            for(int y = 0; y < 10; y++)
            {
                grid.GetCellObject(new CellNumber(x, y)).ResetCell();
            }
        }

        int n = container.transform.childCount;

        for(int i = 0; i < container.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
    }

    public void LoadItem(ItemData data)
    {
        GUI_Item gui = itemInstantiateEvent?.Invoke(data, container);
        List<CellNumber> cellNumsList = GetCellNumList(data.Address, data.Direction, data.Object.Width, data.Object.Height);

        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(cellNumsList[i]);
            //originCellにStackしていく
            if(cellNumsList[i] == data.Address)
            {
                cellObject.InsertItem(gui, data.StackingNum);
                cellObject.SetStack();
            }
            cellObject.Origin = data.Address;
        }

        Vector3 newPosition = grid.GetCellOriginAnchoredPosition(data.Address);

        gui.SetBelongings(this, data.Address, data.Direction);
        gui.RectTransform.SetParent(container);
        gui.SetPivot(data.Direction);
        gui.SetAnchorPosition(newPosition);
        gui.SetRotation(data.Direction);
        gui.SetImageSize(_cellSize);
    }

    public uint InsertItem(GUI_Item gui, CellNumber originCellNum, ItemData.ItemDir direction)
    {
        List<CellNumber> cellNumsList = GetCellNumList(originCellNum, direction, gui.Data.Object.Width, gui.Data.Object.Height);

        uint remain = 0;
        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(cellNumsList[i]);
            //originCellにStackしていく
            if(cellNumsList[i] == originCellNum)
            {
                cellObject.InsertItem(gui, gui.Data.StackingNum);
                cellObject.SetStack();
            }
            cellObject.Origin = originCellNum;
        }

        Vector3 newPosition = grid.GetCellOriginAnchoredPosition(originCellNum);

        gui.SetBelongings(this, originCellNum, direction);
        gui.RectTransform.SetParent(container);
        gui.SetPivot(direction);
        gui.SetAnchorPosition(newPosition);
        gui.SetRotation(direction);
        gui.SetImageSize(_cellSize);

        _openningStorage.AddItem(gui.Data);

        return remain;
    }

    public bool CanPlaceItem(GUI_Item gui, CellNumber originCellNum, ItemData.ItemDir direction)
    {
        List<CellNumber> cellNumsList = GetCellNumList(originCellNum, direction, gui.Data.Object.Width, gui.Data.Object.Height);

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
            if(!originCell.CheckEquality(gui.Data))
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

    public void RemoveItem(CellNumber originCellNum)
    {
        GUI_Item gui = grid.GetCellObject(originCellNum).GUIInCell;
        ItemData.ItemDir direction = gui.Data.Direction;
        List<CellNumber> removeCellNumList = GetCellNumList(originCellNum, direction, gui.Data.Object.Width, gui.Data.Object.Height);
        for(int i = 0; i < removeCellNumList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(removeCellNumList[i]);

            cellObject.ResetCell();
        }

        _openningStorage.TakeItem(gui.Data);
    }

    public CellNumber ScreenPosToCellNum(Vector2 pos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, pos, null, out Vector2 convertPosition);
        return grid.GetCellNum(convertPosition);
    }

    public bool IsValid(CellNumber cellNum)
    {
        return grid.IsValidCellNum(cellNum);
    }

    public List<CellNumber> GetCellNumList(CellNumber originCellNum, ItemData.ItemDir itemDirection, uint width, uint height) 
    {
        List<CellNumber> gridPositionList = new List<CellNumber>();

        switch (itemDirection)
        {
            default:
            case ItemData.ItemDir.Down:
                for (int x = 0; x < width; x++) 
                {
                    for (int y = 0; y < height; y++) 
                    {
                        gridPositionList.Add(originCellNum + new CellNumber(x, y));
                    }
                }
                break;
            case ItemData.ItemDir.Right:
                for (int x = 0; x < height; x++) 
                {
                    for (int y = 0; y < width; y++)
                    {
                        gridPositionList.Add(originCellNum + new CellNumber(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }
}
