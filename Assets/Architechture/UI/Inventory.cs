using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    }

    public void OpenInventory(Storage storage)
    {
        foreach(ItemData data in storage.ItemList)
        {
            GUI_Item gui = Instantiate(GUI_Item_Prefab);
            gui.OnSetUp(data);
            gui.SetImageSize(_cellSize);

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
        gui.RectTransform.SetParent(container);
        gui.SetPivot(direction);
        gui.SetAnchorPosition(newPosition);
        gui.SetImageSize(_cellSize);

        Debug.Log("入れた");
    }

    public CellNumber ScreenPosToCellNum(Vector2 pos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, pos, null, out Vector2 convertPosition);
        return grid.GetCellNum(convertPosition);
    }
}
