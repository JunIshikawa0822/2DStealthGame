using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GUI_Item : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform _rectTransform;
    private ItemData _itemData;
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
    private Inventory _belongingInventory;
    public RectTransform RectTransform {get => _rectTransform;}
    public ItemData Data {get => _itemData;}
    public Inventory BelongingInventory{get => _belongingInventory; set => _belongingInventory = value;}
    public event Action<GUI_Item> onPointerDownEvent;
    public event Action<GUI_Item> onBeginDragEvent;
    public event Action<GUI_Item> onEndDragEvent;
    public event Action<GUI_Item> onDragEvent;

    public event Action<GUI_Item> onUseEvent;

    public void OnSetUp(ItemData itemData)
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _itemData = itemData;

        BackGroundInit();
        _useButton.gameObject.SetActive(false);
        _itemImage.sprite = itemData.Object.ItemImage;
    }

    public void DataSet(ItemData itemData)
    {
        _itemData = itemData;
    }

    public void SetRotation(ItemData.ItemDir itemDirection)
    {
        switch (itemDirection) 
        {
            default:
            case ItemData.ItemDir.Down : 
                _rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case ItemData.ItemDir.Right:  
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
        _rectTransform.sizeDelta = new Vector2(cellSize * _itemData.Object.Width, cellSize * _itemData.Object.Height);
    }

    public void SetPivot(ItemData.ItemDir direction)
    {
        switch(direction)
        {
            case ItemData.ItemDir.Down : 
                _rectTransform.pivot = new Vector2(0, 1);
                Debug.Log(_rectTransform.pivot);
                break;
            case ItemData.ItemDir.Right : 
                _rectTransform.pivot = new Vector2(1, 1);
                Debug.Log(_rectTransform.pivot);
                break;
            case ItemData.ItemDir.Up : 
                _rectTransform.pivot = new Vector2(1, 0);
                Debug.Log(_rectTransform.pivot);
                break;
            case ItemData.ItemDir.Left : 
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
        uint occupyCellNum = _itemData.Object.Width * _itemData.Object.Height;
        for(int i = 0; i < occupyCellNum; i++)
        {
            Instantiate(_backGroundPrefab, _backGroundObject.transform);
        }

        _backGroundObject.SetActive(false);
    }

    public void SetBelongings(Inventory inventory, CellNumber originCellNum, ItemData.ItemDir direction)
    {
        _belongingInventory = inventory;
        _itemData.Address = originCellNum;
        _itemData.Direction = direction;
    }

    public void OnDestroy()
    {
        Destroy(this.gameObject);
    }

    public ItemData.ItemDir GetNextDir(ItemData.ItemDir dir)
    {
        switch (dir) {
            default:
            case ItemData.ItemDir.Down:  return ItemData.ItemDir.Right;
            case ItemData.ItemDir.Right:  return ItemData.ItemDir.Down;
        }
    }
    public int GetRotationAngle(ItemData.ItemDir itemDirection)
    {
        switch (itemDirection) 
        {
            default : return 0;
            case ItemData.ItemDir.Down : return 0;
            case ItemData.ItemDir.Right: return 90;
            case ItemData.ItemDir.Up   : return 180;
            case ItemData.ItemDir.Left : return 270;
        }
    }
    // public List<CellNumber> GetCellNumList(CellNumber originCellNum, ItemData.ItemDir itemDirection) 
    // {
    //     List<CellNumber> gridPositionList = new List<CellNumber>();

    //     switch (itemDirection)
    //     {
    //         default:
    //         case ItemData.ItemDir.Down:
    //             for (int x = 0; x < _itemData.Object.Width; x++) 
    //             {
    //                 for (int y = 0; y < _itemData.Object.Height; y++) 
    //                 {
    //                     gridPositionList.Add(originCellNum + new CellNumber(x, y));
    //                 }
    //             }
    //             break;
    //         case ItemData.ItemDir.Right:
    //             for (int x = 0; x < _itemData.Object.Height; x++) 
    //             {
    //                 for (int y = 0; y < _itemData.Object.Width; y++) 
    //                 {
    //                     gridPositionList.Add(originCellNum + new CellNumber(x, y));
    //                 }
    //             }
    //             break;
    //     }
    //     return gridPositionList;
    // }
    public CellNumber[] GetOccupyCells(ItemData.ItemDir itemDirection, CellNumber originCellNum)
    {
        CellNumber[] gridPositionsArray = new CellNumber[_itemData.Object.Width * _itemData.Object.Height];
        switch (itemDirection)
        {
            default:
            case ItemData.ItemDir.Down:
                for (int x = 0; x < _itemData.Object.Width; x++) 
                {
                    for (int y = 0; y < _itemData.Object.Height; y++)
                    {
                        gridPositionsArray[_itemData.Object.Height * x + y] = originCellNum + new CellNumber(x, y);
                    }
                }
                break;
            case ItemData.ItemDir.Right:
                for (int x = 0; x < _itemData.Object.Height; x++)
                {
                    for (int y = 0; y < _itemData.Object.Width; y++)
                    {
                        gridPositionsArray[_itemData.Object.Width * x + y] = originCellNum + new CellNumber(x, y);
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
        if(_itemData.Object.IsClickUse)_useButton.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(_itemData.Object.IsClickUse)_useButton.gameObject.SetActive(false);
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
