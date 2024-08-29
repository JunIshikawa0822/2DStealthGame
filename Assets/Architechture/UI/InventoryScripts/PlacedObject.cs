using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PlacedObject : ItemDragAndDrop<PlacedObject>
{
    private Scriptable_UI_Item itemData;
    private Scriptable_UI_Item.ItemDir itemDirection;

    private RectTransform rectTransform;

    private TetrisInventory belongingInventory;
    private Vector2Int belongingCellNum;
    private GridLayoutGroup backGroundGrid;
    //透過率調整用
    private CanvasGroup canvasGroup;

    [SerializeField]
    private GameObject backGround;

    [SerializeField]
    private TextMeshProUGUI stackNumText;

    private int stackNum;

    public void OnSetUp(Scriptable_UI_Item itemData, int stackNum)
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        backGroundGrid = backGround.GetComponent<GridLayoutGroup>();

        this.itemData = itemData;
        this.itemDirection = itemData.direction;
        this.stackNum = stackNum;
        Tobject = this;

        TextSet();
    }

    public void BelongingsInit(TetrisInventory belongingInventory, Vector2Int belongingCellNum, Scriptable_UI_Item.ItemDir direction, RectTransform parent)
    {
        this.belongingInventory = belongingInventory;
        this.belongingCellNum = belongingCellNum;
        this.itemDirection = direction;
        this.rectTransform.SetParent(parent);
    }

    public void ImageSizeSet(float cellSize)
    {
        rectTransform.sizeDelta = new Vector2(itemData.width, itemData.height) * cellSize;
        backGroundGrid.cellSize = new Vector2(cellSize, cellSize);
    }

    public void TextSet()
    {
        stackNumText.text = stackNum.ToString();
    }

    public Scriptable_UI_Item GetItemData()
    {
        return itemData;
    }

    public Scriptable_UI_Item.ItemDir GetDirection()
    {
        return this.itemDirection;
    }

    public int GetStackNum()
    {
        return this.stackNum;
    }

    public void OnDragStart()
    {
        backGround.SetActive(true);
        canvasGroup.alpha = 0.7f;  
    }

    public void OnDragEnd()
    {
        backGround.SetActive(false);
        canvasGroup.alpha = 1f;
    }

    public RectTransform GetRectTransform()
    {
        return this.rectTransform;
    }

    public TetrisInventory GetBelongingInventory()
    {
        return this.belongingInventory;
    }

    public Vector2Int GetBelongingCellNum()
    {
        return this.belongingCellNum;
    }
}
