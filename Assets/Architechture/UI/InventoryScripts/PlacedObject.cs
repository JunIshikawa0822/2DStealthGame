using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PlacedObject : ItemDragAndDrop<PlacedObject>
{
    private Scriptable_UI_Item _itemData;
    private Scriptable_UI_Item.ItemDir _itemDirection;
    private RectTransform _rectTransform;
    private TetrisInventory _belongingInventory;
    private Vector2Int _belongingCellNum;
    private GridLayoutGroup backGroundGrid;
    //透過率調整用
    private CanvasGroup canvasGroup;
    [SerializeField]
    private GameObject backGround;
    [SerializeField]
    private TextMeshProUGUI _stackingNumText;
    private int _stackingNum;

    public void OnSetUp(Scriptable_UI_Item itemData)
    {
        _rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        backGroundGrid = backGround.GetComponent<GridLayoutGroup>();

        _itemData = itemData;
        _itemDirection = Scriptable_UI_Item.ItemDir.Down;
       
        BackGroundInit();
    }

    public void StackNumInit(int stackNum)
    {
        _stackingNum = stackNum;
        TextSet();
    }

    public void SetRotation(Quaternion quaternion)
    {
        _rectTransform.rotation = quaternion;
    }

    private void BackGroundInit()
    {
        int occupyCellNum = _itemData.width * _itemData.height;
        RectTransform backGroundTransform = backGround.GetComponent<RectTransform>();
        
        for(int i = 0; i < occupyCellNum; i++)
        {
            Instantiate(_itemData.backGroundPrefab, backGroundTransform);
        }
    }

    public void SetBelonging(TetrisInventory belongingInventory, Vector2Int belongingCellNum, Scriptable_UI_Item.ItemDir direction, RectTransform parent)
    {
        _belongingInventory = belongingInventory;
        _belongingCellNum = belongingCellNum;
        _itemDirection = direction;
        _rectTransform.SetParent(parent);
    }

    public void ImageSizeSet(float cellSize)
    {
        _rectTransform.sizeDelta = new Vector2(_itemData.width, _itemData.height) * cellSize;
        backGroundGrid.cellSize = new Vector2(cellSize, cellSize);
    }

    public void TextSet()
    {
        _stackingNumText.text = _stackingNum.ToString();
    }

    public Scriptable_UI_Item GetItemData(){return _itemData;}
    public Scriptable_UI_Item.ItemDir GetDirection(){return _itemDirection;}
    public RectTransform GetRectTransform(){return _rectTransform;}
    public TetrisInventory GetBelongingInventory(){return _belongingInventory;}
    public Vector2Int GetBelongingCellNum(){return _belongingCellNum;}

    public int GetStackNum(){return _stackingNum;}

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

    public void OnDestroy()
    {
        Destroy(this.gameObject);
    }
}
