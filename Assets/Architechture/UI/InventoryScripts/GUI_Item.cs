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
    private AInventory _belongingInventory;
    public RectTransform RectTransform {get => _rectTransform;}
    public ItemData Data {get => _itemData;}
    public AInventory BelongingInventory{get => _belongingInventory; set => _belongingInventory = value;}
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
        _itemImage.sprite = itemData.ObjectData.ItemImage;
    }

    public void DataSet(ItemData itemData)
    {
        _itemData = itemData;
    }

    public void SetStackNum(uint stackNum)
    {
        _itemData.StackingNum = stackNum;
        _stackingNumText.text = _itemData.StackingNum.ToString();
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
        _rectTransform.sizeDelta = new Vector2(cellSize * _itemData.ObjectData.Width, cellSize * _itemData.ObjectData.Height);
    }

    public void SetPivot(ItemData.ItemDir direction)
    {
        switch(direction)
        {
            case ItemData.ItemDir.Down : 
                _rectTransform.pivot = new Vector2(0, 1);
                // Debug.Log(_rectTransform.pivot);
                break;
            case ItemData.ItemDir.Right : 
                _rectTransform.pivot = new Vector2(1, 1);
                // Debug.Log(_rectTransform.pivot);
                break;
            case ItemData.ItemDir.Up : 
                _rectTransform.pivot = new Vector2(1, 0);
                // Debug.Log(_rectTransform.pivot);
                break;
            case ItemData.ItemDir.Left : 
                _rectTransform.pivot = new Vector2(0, 0);
                //Debug.Log(_rectTransform.pivot);
                break;
            default :
                _rectTransform.pivot = new Vector2(0.5f, 0.5f);
                break;
        }
    }

    private void BackGroundInit()
    {
        uint occupyCellNum = _itemData.ObjectData.Width * _itemData.ObjectData.Height;
        for(int i = 0; i < occupyCellNum; i++)
        {
            Instantiate(_backGroundPrefab, _backGroundObject.transform);
        }

        _backGroundObject.SetActive(false);
    }

    public void SetInventory(AInventory inventory)
    {
        _belongingInventory = inventory;
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
        CellNumber[] gridPositionsArray = new CellNumber[_itemData.ObjectData.Width * _itemData.ObjectData.Height];
        switch (itemDirection)
        {
            default:
            case ItemData.ItemDir.Down:
                for (int x = 0; x < _itemData.ObjectData.Width; x++) 
                {
                    for (int y = 0; y < _itemData.ObjectData.Height; y++)
                    {
                        gridPositionsArray[_itemData.ObjectData.Height * x + y] = originCellNum + new CellNumber(x, y);
                    }
                }
                break;
            case ItemData.ItemDir.Right:
                for (int x = 0; x < _itemData.ObjectData.Height; x++)
                {
                    for (int y = 0; y < _itemData.ObjectData.Width; y++)
                    {
                        gridPositionsArray[_itemData.ObjectData.Width * x + y] = originCellNum + new CellNumber(x, y);
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
        if(_itemData.ObjectData.IsClickUse)_useButton.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(_itemData.ObjectData.IsClickUse)_useButton.gameObject.SetActive(false);
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
