using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Item_GUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemDir _itemDirection;
    private RectTransform _rectTransform;
    private IInventory _belongingInventory;
    private CellNumber _belongingCellNum;
    private Vector3 _belongingPosition;

    public uint StackingNum{get; set;}
    private A_Item_Data _itemData;
    [SerializeField]
    private GameObject _backGroundObject;
    [SerializeField]
    private GameObject _backGroundPrefab;
    [SerializeField]
    private Image _itemImage;

    [SerializeField]
    private Image _useButton;
    //透過率調整用
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private TextMeshProUGUI _stackingNumText;

    public ItemDir ItemDirection {get => _itemDirection;}
    public RectTransform RectTransform {get => _rectTransform;}
    public IInventory BelongingInventory {get => _belongingInventory;}
    public CellNumber BelongingCellNum {get => _belongingCellNum;}
    public A_Item_Data ItemData {get => _itemData;}

    public event Action<Item_GUI> onPointerDownEvent;
    public event Action<Item_GUI> onBeginDragEvent;
    public event Action<Item_GUI> onEndDragEvent;
    public event Action<Item_GUI> onDragEvent;

    public event Action<Item_GUI> onUseEvent;
    
    public enum ItemDir
    {
        Down,
        Right,
        Up,
        Left,
        Middle
    }

    public void OnSetUp(A_Item_Data itemData)
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _itemData = itemData;
        // Debug.Log(_itemData.widthInGUI);
        // Debug.Log(_itemData.heightInGUI);

        _itemDirection = ItemDir.Down;
        _itemImage.sprite = itemData.ItemImage;
        BackGroundInit();

        _useButton.gameObject.SetActive(false);
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

    public void SetImageSize(float cellSize)
    {
        _rectTransform.sizeDelta = new Vector2(cellSize * _itemData.Width, cellSize * _itemData.Height);
    }

    public void SetPivot(ItemDir direction)
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
                _rectTransform.pivot = new Vector2(0.5f, 0.5f);
                break;
        }
    }

    private void BackGroundInit()
    {
        uint occupyCellNum = _itemData.Width * _itemData.Height;
        for(int i = 0; i < occupyCellNum; i++)
        {
            Instantiate(_backGroundPrefab, _backGroundObject.transform);
        }

        _backGroundObject.SetActive(false);
    }

    public void SetBelongings(IInventory belongingInventory, Vector3 pos, ItemDir direction)
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
            default : return 0;
            case ItemDir.Down : return 0;
            case ItemDir.Right: return 90;
            case ItemDir.Up   : return 180;
            case ItemDir.Left : return 270;
        }
    }

    public List<CellNumber> GetCellNumList(CellNumber originCellNum, ItemDir itemDirection) 
    {
        List<CellNumber> gridPositionList = new List<CellNumber>();

        switch (itemDirection)
        {
            default:
            case ItemDir.Down:
                for (int x = 0; x < _itemData.Width; x++) 
                {
                    for (int y = 0; y < _itemData.Height; y++) 
                    {
                        gridPositionList.Add(originCellNum + new CellNumber(x, y));
                    }
                }
                break;
            case ItemDir.Right:
                for (int x = 0; x < _itemData.Height; x++) 
                {
                    for (int y = 0; y < _itemData.Width; y++) 
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
        CellNumber[] gridPositionsArray = new CellNumber[_itemData.Width * _itemData.Height];
        switch (itemDirection)
        {
            default:
            case ItemDir.Down:
                for (int x = 0; x < _itemData.Width; x++) 
                {
                    for (int y = 0; y < _itemData.Height; y++)
                    {
                        gridPositionsArray[_itemData.Height * x + y] = originCellNum + new CellNumber(x, y);
                    }
                }
                break;
            case ItemDir.Right:
                for (int x = 0; x < _itemData.Height; x++)
                {
                    for (int y = 0; y < _itemData.Width; y++)
                    {
                        gridPositionsArray[_itemData.Width * x + y] = originCellNum + new CellNumber(x, y);
                    }
                }
                break;
        }
        return gridPositionsArray;
    }

    public void OnUse()
    {
        if(onUseEvent == null)return;
        onUseEvent.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(_itemData.IsClickUse)_useButton.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(_itemData.IsClickUse)_useButton.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(onPointerDownEvent == null)return;
        onPointerDownEvent.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        if(onBeginDragEvent == null)return;
        _useButton.gameObject.SetActive(false);
        _backGroundObject.SetActive(true);
        _canvasGroup.alpha = 0.8f;
        onBeginDragEvent.Invoke(this);
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        if(onEndDragEvent == null)return;
        _useButton.gameObject.SetActive(false);
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
