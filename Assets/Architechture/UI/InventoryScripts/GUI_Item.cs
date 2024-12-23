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

    #region  計算用

    private AInventory _toInventory;
    private AInventory _fromInventory;

    private float _oldAngle;
    private float _newAngle;
    private float _rotateAngle;
    //private GUI_Item _draggingObject;

    private bool _isDragging;
    private ItemData.ItemDir _oldDirection;
    private ItemData.ItemDir _newDirection;

    private Vector3[] _offsetArray = new Vector3[4];
    private Vector3 _positionOffset;
    //private Vector3 _oldPosition;

    //------------------
    private CellNumber _oldCellNum;
    #endregion

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

    public void StartDragging()
    {
        //if(!gameStat.isInventoryPanelActive)return;
        //if(gui == null)return

        _isDragging = true;

        _oldDirection = _newDirection = Data.Direction;
        _fromInventory =  _belongingInventory;
        //_oldPosition = gui.RectTransform.position;
        _oldCellNum = Data.Address;

        _rotateAngle = 0;

        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        RectTransform.GetWorldCorners(corners);

        //GetWorldCornersはCanvasのRenderModeによって変わるらしい
        //ScreenSpace OverlayならそのままScreen座標
        //ScreenSpace Camera、World Spaceなら、その後WorldToScreenPosで変換する必要がある
        //GetWorldCornersは、左下、左上、右上、右下の順で格納してしまうので、ItemDirの並び順と揃える必要がある

        for(int i = 0; i < corners.Length; i++)
        {
            //Debug.Log((i + 1) % 4);
            _offsetArray[i] = corners[(i + 1) % 4] - mousePos;

            //Debug.Log($"{corners[(i + 1) % 4]} : {_offsetArray[i].magnitude}");
            //Debug.Log($"{corners[i]} : {corners[i].magnitude}");
        }

        // Debug.Log(_oldDirection);
        _positionOffset = _offsetArray[(int)_oldDirection];
        //transform.SetParent(_UGUIPanel.transform);
        SetPivot(_oldDirection);
    }

    public void EndDragging(GUI_Item gui)
    {
        //if(!gameStat.isInventoryPanelActive)return;
        
        Vector3 mousePos = Input.mousePosition;

        _fromInventory.RemoveItem(_oldCellNum);

        Vector3 newPosition = mousePos + _positionOffset;

        CellNumber newCell = new CellNumber(0, 0);

        //所属Inventoryを探す
        foreach (AInventory inventory in gameStat.inventoryList)
        {
            newCell = inventory.ScreenPosToCellNum(newPosition);

            Debug.Log(inventory.IsValid(newCell));

            if (inventory.IsValid(newCell))
            {
                _toInventory = inventory;
                break;
            }
        }

        //Debug.Log(newCell);
        //Debug.Log(_toInventory);

        if (_toInventory != null || gui == null)
        {
            if(_toInventory.CanPlaceItem(gui, newCell, _newDirection))
            {
                //Debug.Log("おけてはいる");
                uint remain = _toInventory.InsertItem(gui, newCell, _newDirection);

                if(remain > 0)
                {
                    //remain分はromInventoryに再度格納
                    _fromInventory.InsertItem(gui, _oldCellNum, _oldDirection);
                }
            }
            else
            {
                _fromInventory.InsertItem(gui, _oldCellNum, _oldDirection);
                Debug.Log("置けなかった");
            }
        }
        else
        {
            Debug.Log("toInventory、itemがなかった");
            _fromInventory.InsertItem(gui, _oldCellNum, _oldDirection);
        }

        _draggingObject = null;
        _toInventory = null;
    }
}
