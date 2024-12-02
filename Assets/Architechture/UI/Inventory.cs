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

    public GUI_Item GUI_Item_Prefab;
    private IObjectPool _objectPool;

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
        foreach(ItemData data in storage.ItemList)
        {
            GUI_Item gui = Instantiate(GUI_Item_Prefab);
            Debug.Log(gui.transform.localScale);
            gui.OnSetUp(data);
            //gui.SetImageSize(_cellSize);

            InsertToGrid(gui, data.Address, data.Direction);
        }
    }

    public void CloseInventory()
    {

    }

    public void InsertToGrid(GUI_Item gui, CellNumber originCellNum, ItemData.ItemDir direction)
    {
        ItemData itemData = gui.ItemData;
        List<CellNumber> cellNumsList = gui.GetCellNumList(originCellNum, direction);

        for(int i = 0; i < cellNumsList.Count; i++)
        {
            CellObject cellObject =  grid.GetCellObject(cellNumsList[i]);

            if(cellNumsList[i] == originCellNum)
            {
                cellObject.InsertItem(itemData, itemData.StackNum);
                cellObject.SetStack();
            }

            cellObject.Origin = originCellNum;
        }

        Vector2 newPosition = grid.GetCellOriginAnchoredPosition(originCellNum);
        Debug.Log(gui.transform.localScale);
        gui.RectTransform.SetParent(container);
        Debug.Log(gui.transform.localScale);
        gui.SetPivot(direction);
        Debug.Log(gui.transform.localScale);
        gui.SetAnchorPosition(newPosition);
        Debug.Log(gui.transform.localScale);
        gui.SetImageSize(_cellSize);

        Debug.Log("入れた");
        Debug.Log(gui.transform.localScale);
    }

    public CellNumber ScreenPosToCellNum(Vector2 pos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, pos, null, out Vector2 convertPosition);
        return grid.GetCellNum(convertPosition);
    }
}
