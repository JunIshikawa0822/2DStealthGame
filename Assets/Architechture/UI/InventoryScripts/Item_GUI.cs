using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Item_GUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private ItemDir _itemDirection;
    private RectTransform _rectTransform;
    private TetrisInventory _belongingInventory;
    private CellNumber _belongingCellNum;
    private Vector3 _belongingPosition;
    public uint StackingNum{get; set;}
    private Scriptable_ItemData _itemData;
    [SerializeField]
    private GameObject _backGroundObject;
    [SerializeField]
    private GameObject _backGroundPrefab;
    [SerializeField]
    private Image _itemImage;
    //透過率調整用
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private TextMeshProUGUI _stackingNumText;

    public event Action<Item_GUI> onPointerDownEvent;
    public event Action<Item_GUI> onBeginDragEvent;
    public event Action<Item_GUI> onEndDragEvent;
    public event Action<Item_GUI> onDragEvent;
    
    public enum ItemDir
    {
        Down,
        Right,
        Up,
        Left
    }

    public void OnSetUp(Scriptable_ItemData itemData)
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _itemData = itemData;
        // Debug.Log(_itemData.widthInGUI);
        // Debug.Log(_itemData.heightInGUI);

        _itemDirection = ItemDir.Down;
        _itemImage.sprite = itemData.itemImage;
        BackGroundInit();
    }

#region stackNumここでセットする？
    public void SetStackNum(uint stackNum)
    {
        StackingNum = stackNum;
        _stackingNumText.text = StackingNum.ToString();
    }
#endregion

    public void SetRotation(Item_GUI.ItemDir itemDirection)
    {
        switch (itemDirection) 
        {
            default:
            case ItemDir.Down :  
                _rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case ItemDir.Right:  
                _rectTransform.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }
    }

    public void SetPosition(Vector3 pos)
    {
        _rectTransform.position = pos;
    }

    public void SetAnchorPosition(Vector3 pos)
    {
        _rectTransform.anchoredPosition = pos;
    }

    public void SetAnchor(ItemDir direction)
    {
        switch(direction)
        {
            case ItemDir.Down : 
                _rectTransform.pivot = new Vector2(0, 1);
                Debug.Log(_rectTransform.pivot);
                break;
            case ItemDir.Right : 
                _rectTransform.pivot = new Vector2(1, 1);
                Debug.Log(_rectTransform.pivot);
                break;
            case ItemDir.Up : 
                _rectTransform.pivot = new Vector2(1, 0);
                Debug.Log(_rectTransform.pivot);
                break;
            case ItemDir.Left : 
                _rectTransform.pivot = new Vector2(0, 0);
                Debug.Log(_rectTransform.pivot);
                break;
            default :
                _rectTransform.pivot = new Vector2(0, 1);
                break;
        }
    }

    private void BackGroundInit()
    {
        uint occupyCellNum = _itemData.widthInGUI * _itemData.heightInGUI;
        for(int i = 0; i < occupyCellNum; i++)
        {
            Instantiate(_backGroundPrefab, _backGroundObject.transform);
        }

        _backGroundObject.SetActive(false);
    }

    public void ImageInit(float cellSize)
    {
        _rectTransform.sizeDelta = new Vector2(_itemData.widthInGUI, _itemData.heightInGUI) * cellSize;
        _backGroundObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
    }

    public void SetBelongings(TetrisInventory belongingInventory, CellNumber belongingCellNum, ItemDir direction, RectTransform parent)
    {
        _belongingInventory = belongingInventory;
        _belongingCellNum = belongingCellNum;
        _itemDirection = direction;
        _rectTransform.SetParent(parent);
    }

    public void SetBelongings(TetrisInventory belongingInventory, Vector3 pos, ItemDir direction)
    {
        _belongingInventory = belongingInventory;
        _belongingPosition = pos;
        _itemDirection = direction;
    }

    public void OnDestroy()
    {
        Destroy(this.gameObject);
    }

    //public Scriptable_UI_Item GetItemData(){return _itemData;}
    public ItemDir GetDirection(){return _itemDirection;}
    public RectTransform GetRectTransform(){return _rectTransform;}
    public TetrisInventory GetBelongingInventory(){return _belongingInventory;}
    public CellNumber GetBelongingCellNum(){return _belongingCellNum;}
    public Scriptable_ItemData GetItemData(){return _itemData;}
    //public uint GetStackNum(){return _stackingNum;}

    public ItemDir GetNextDir(ItemDir dir) 
    {
        switch (dir) {
            default:
            case ItemDir.Down:  return ItemDir.Right;
            case ItemDir.Right:  return ItemDir.Down;
        }
    }

    public int GetRotationAngle(ItemDir itemDirection)
    {
        switch (itemDirection) 
        {
            default:
            case ItemDir.Down : return 0;
            case ItemDir.Right: return 90;
            case ItemDir.Up   : return 180;
            case ItemDir.Left : return 270;
        }
    }

    public CellNumber GetRotationOffset(ItemDir itemDirection) 
    {
        switch (itemDirection)
        {
            default:
            case ItemDir.Down:  return new CellNumber(0, 0);
            case ItemDir.Right:  return new CellNumber((int)_itemData.heightInGUI, 0);
        }
    }

    public ItemDir GetEnumByIndex(int index)
    {
        ItemDir[] directions = (ItemDir[])Enum.GetValues(typeof(ItemDir));
        if (index >= 0 && index < directions.Length)
        {
            return directions[index];
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(index), "インデックスが範囲外です");
        }
    }

    public CellNumber GetRotatedCellNumOffset(ItemDir originDirection, ItemDir newDirection, CellNumber offset)
    {
        int rest_x = (int)(_itemData.widthInGUI - 1) - offset.x;
        int rest_y = (int)(_itemData.heightInGUI - 1) - offset.y;

        switch(originDirection)
        {
            default:
            case ItemDir.Right:
            //case ItemDir.Right: 
                rest_x = (int)(_itemData.heightInGUI - 1) - offset.x;
                rest_y = (int)(_itemData.widthInGUI - 1) - offset.y;
                break;

            case ItemDir.Down: 
            //case ItemDir.Up:
                rest_x = (int)(_itemData.widthInGUI - 1) - offset.x;
                rest_y = (int)(_itemData.heightInGUI - 1) - offset.y;
                break;
        }

        CellNumber rotateOffset = new CellNumber(offset.x, offset.y);

        if(originDirection == newDirection)
        {
            rotateOffset = new CellNumber(offset.x, offset.y);
            Debug.Log("same");
        }
        else if(newDirection == ItemDir.Down)
        {
            rotateOffset = new CellNumber(offset.y, rest_y);
            Debug.Log("Down");
        }
        else
        {
            rotateOffset = new CellNumber(offset.y, rest_x);
            // Debug.Log("Left");
        }

        // Debug.Log("でてくるoffset : " + rotateOffset);
        return rotateOffset;
    }

    // public Vector2Int GetCellNumRotateOffset(ItemDir originDirection, ItemDir itemDirection, Vector2Int offset)
    // {
    //     int rest_x = (_width - 1) - offset.x;
    //     int rest_y = (_height - 1) - offset.y;

    //     switch(originDirection)
    //     {
    //         default:
    //         case ItemDir.Left:  
    //         //case ItemDir.Right: 
    //             rest_x = (_height - 1) - offset.x;
    //             rest_y = (_width - 1) - offset.y;
    //             break;

    //         case ItemDir.Down: 
    //         //case ItemDir.Up:
    //             rest_x = (_width - 1) - offset.x;
    //             rest_y = (_height - 1) - offset.y;
    //             break;
    //     }

    //     Vector2Int rotateOffset = new Vector2Int(offset.x, offset.y);

    //     if(itemDirection == originDirection)
    //     {
    //         rotateOffset = new Vector2Int(offset.x, offset.y);
    //         // Debug.Log("same");
    //     }
    //     else if(itemDirection == GetNextDir(originDirection))
    //     {
    //         rotateOffset = new Vector2Int(rest_y, offset.x);
    //         // Debug.Log("next");
    //     }
    //     else if(itemDirection == GetNextDir(GetNextDir(originDirection)))
    //     {
    //         rotateOffset = new Vector2Int(rest_x, rest_y);
    //         // Debug.Log("next next");
    //     }
    //     else
    //     {
    //         rotateOffset = new Vector2Int(offset.y, rest_x);
    //         // Debug.Log("previous");
    //     }

    //     // Debug.Log("でてくるoffset : " + rotateOffset);
    //     return rotateOffset;
    // }

    public List<CellNumber> GetCellNumList(CellNumber originCellNum, ItemDir itemDirection) 
    {
        List<CellNumber> gridPositionList = new List<CellNumber>();

        switch (itemDirection)
        {
            default:
            case ItemDir.Down:
                for (int x = 0; x < _itemData.widthInGUI; x++) 
                {
                    for (int y = 0; y < _itemData.heightInGUI; y++) 
                    {
                        gridPositionList.Add(originCellNum + new CellNumber(x, y));
                    }
                }
                break;
            case ItemDir.Right:
                for (int x = 0; x < _itemData.heightInGUI; x++) 
                {
                    for (int y = 0; y < _itemData.widthInGUI; y++) 
                    {
                        gridPositionList.Add(originCellNum + new CellNumber(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }

    public CellNumber[] GetOccupyCells(ItemDir itemDirection, CellNumber originCellNum)
    {
        CellNumber[] gridPositionsArray = new CellNumber[_itemData.widthInGUI * _itemData.heightInGUI];
        switch (itemDirection)
        {
            default:
            case ItemDir.Down:
                for (int x = 0; x < _itemData.widthInGUI; x++) 
                {
                    for (int y = 0; y < _itemData.heightInGUI; y++)
                    {
                        gridPositionsArray[_itemData.heightInGUI * x + y] = originCellNum + new CellNumber(x, y);
                    }
                }
                break;
            case ItemDir.Right:
                for (int x = 0; x < _itemData.heightInGUI; x++)
                {
                    for (int y = 0; y < _itemData.widthInGUI; y++)
                    {
                        gridPositionsArray[_itemData.widthInGUI * x + y] = originCellNum + new CellNumber(x, y);
                    }
                }
                break;
        }
        return gridPositionsArray;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(onPointerDownEvent == null)return;
        onPointerDownEvent.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        if(onBeginDragEvent == null)return;
        _backGroundObject.SetActive(true);
        _canvasGroup.alpha = 0.8f;
        onBeginDragEvent.Invoke(this);
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        if(onEndDragEvent == null)return;
        _backGroundObject.SetActive(false);
        _canvasGroup.alpha = 1f;
        onEndDragEvent.Invoke(this);
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        if(onDragEvent == null)return;
        onDragEvent.Invoke(this);
    }
}
