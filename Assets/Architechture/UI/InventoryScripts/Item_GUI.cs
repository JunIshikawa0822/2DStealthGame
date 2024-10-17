using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Item_GUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private int _width;
    private int _height;
    private ItemDir _itemDirection;
    private RectTransform _rectTransform;
    private TetrisInventory _belongingInventory;
    private Vector2Int _belongingCellNum;

    private int _stackingNum;
    private string _itemDataID;
    //private Scriptable_ItemData _itemData;
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
        Left
    }

    public void OnSetUp(Scriptable_ItemData itemData)
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _itemDirection = ItemDir.Down;
        _width = itemData.widthInGUI;
        _height = itemData.heightInGuI;
        _itemImage.sprite = itemData.itemImage;
        BackGroundInit();
    }

    public void StackInit(int stackNum)
    {
        _stackingNum = stackNum;
        _stackingNumText.text = _stackingNum.ToString();
    }

    public void SetRotation(Quaternion quaternion)
    {
        _rectTransform.rotation = quaternion;
    }

    public void SetPosition(Vector3 pos)
    {
        _rectTransform.position = pos;
    }

    public void SetAnchorPosition(Vector3 pos)
    {
        _rectTransform.anchoredPosition = pos;
    }

    private void BackGroundInit()
    {
        int occupyCellNum = _width * _height;
        for(int i = 0; i < occupyCellNum; i++)
        {
            Instantiate(_backGroundPrefab, _backGroundObject.transform);
        }

        _backGroundObject.SetActive(false);
    }

    public void ImageInit(float cellSize)
    {
        _rectTransform.sizeDelta = new Vector2(_width, _height) * cellSize;
        _backGroundObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
    }

    public void SetBelonging(TetrisInventory belongingInventory, Vector2Int belongingCellNum, ItemDir direction, RectTransform parent)
    {
        _belongingInventory = belongingInventory;
        _belongingCellNum = belongingCellNum;
        _itemDirection = direction;
        _rectTransform.SetParent(parent);
    }

    //public Scriptable_UI_Item GetItemData(){return _itemData;}
    public ItemDir GetDirection(){return _itemDirection;}
    // public RectTransform GetRectTransform(){return _rectTransform;}
    public TetrisInventory GetBelongingInventory(){return _belongingInventory;}
    public Vector2Int GetBelongingCellNum(){return _belongingCellNum;}
    public int GetStackNum(){return _stackingNum;}

    public ItemDir GetNextDir(ItemDir dir) 
    {
        switch (dir) {
            default:
            case ItemDir.Down:  return ItemDir.Left;
            case ItemDir.Left:  return ItemDir.Down;
        }
    }

    public int GetRotationAngle(ItemDir itemDirection)
    {
        switch (itemDirection) 
        {
            default:
            case ItemDir.Down :  return 0;
            case ItemDir.Left:  return 90;
        }
    }

    public Vector2Int GetRotationOffset(ItemDir itemDirection) 
    {
        switch (itemDirection)
        {
            default:
            case ItemDir.Down:  return new Vector2Int(0, 0);
            case ItemDir.Left:  return new Vector2Int(_height, 0);
        }
    }

    public Vector2Int GetCellNumRotateOffset(ItemDir originDirection, ItemDir itemDirection, Vector2Int offset)
    {
        int rest_x = (_width - 1) - offset.x;
        int rest_y = (_height - 1) - offset.y;

        switch(originDirection)
        {
            default:
            case ItemDir.Left:  
            //case ItemDir.Right: 
                rest_x = (_height - 1) - offset.x;
                rest_y = (_width - 1) - offset.y;
                break;

            case ItemDir.Down: 
            //case ItemDir.Up:
                rest_x = (_width - 1) - offset.x;
                rest_y = (_height - 1) - offset.y;
                break;
        }
        Vector2Int rotateOffset = new Vector2Int(offset.x, offset.y);

        if(itemDirection == originDirection)
        {
            rotateOffset = new Vector2Int(offset.x, offset.y);
            // Debug.Log("same");
        }
        else if(itemDirection == GetNextDir(originDirection))
        {
            rotateOffset = new Vector2Int(rest_y, offset.x);
            // Debug.Log("next");
        }
        else if(itemDirection == GetNextDir(GetNextDir(originDirection)))
        {
            rotateOffset = new Vector2Int(rest_x, rest_y);
            // Debug.Log("next next");
        }
        else
        {
            rotateOffset = new Vector2Int(offset.y, rest_x);
            // Debug.Log("previous");
        }

        // Debug.Log("でてくるoffset : " + rotateOffset);
        return rotateOffset;
    }

    public List<Vector2Int> GetCellNumList(ItemDir itemDirection, Vector2Int originCellNum) 
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        switch (itemDirection) 
        {
            default:
            case ItemDir.Down:
                for (int x = 0; x < _width; x++) 
                {
                    for (int y = 0; y < _height; y++) 
                    {
                        gridPositionList.Add(originCellNum + new Vector2Int(x, y));
                    }
                }
                break;
            case ItemDir.Left:
                for (int x = 0; x < _height; x++) 
                {
                    for (int y = 0; y < _width; y++) 
                    {
                        gridPositionList.Add(originCellNum + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
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
        _canvasGroup.alpha = 1f;
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
