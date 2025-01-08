using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityDebugSheet.Runtime.Core.Scripts;

public class Item_GUI : A_Item_GUI
{
    IInventoryItem _inventoryItem;
    //private A_Item_Data _itemData;
    [SerializeField]
    private GameObject _backGroundObject;
    [SerializeField]
    private GameObject _backGroundPrefab;
    [SerializeField]
    private Image _itemImage;
    [SerializeField]
    private Image _useButton;
    [SerializeField]
    private TextMeshProUGUI _stackingNumText;

        //透過率調整用
    private CanvasGroup _canvasGroup;

    public override void OnSetUp()
    {
        base.OnSetUp();

        _canvasGroup = GetComponent<CanvasGroup>();
        BackGroundInit();
        _useButton.gameObject.SetActive(false);
    }

    public override void Init(IInventoryItem inventoryItem)
    {
        _inventoryItem = inventoryItem;
        _itemImage.sprite = inventoryItem.Data.ItemImage;
    }

    public override void SetNewStatus(CellNumber newAddress, IInventoryItem.ItemDir newDir)
    {
        _inventoryItem.Address = newAddress;
        _inventoryItem.Direction = newDir;
    }

    public override (CellNumber oldAddress, IInventoryItem.ItemDir oldDir) GetOldStatus()
    {
        return (_inventoryItem.Address, _inventoryItem.Direction);
    }

    public override void SetRotation(IInventoryItem.ItemDir itemDirection)
    {
        switch (itemDirection) 
        {
            default:
            case IInventoryItem.ItemDir.Down : 
                SetRotation(Quaternion.Euler(0, 0, 0));
                break;
            case IInventoryItem.ItemDir.Right:  
                SetRotation(Quaternion.Euler(0, 0, 90));
                break;
        }
    }

    public IInventoryItem.ItemDir GetNextDir(IInventoryItem.ItemDir dir)
    {
        switch (dir) {
            default:
            case IInventoryItem.ItemDir.Down:  return IInventoryItem.ItemDir.Right;
            case IInventoryItem.ItemDir.Right:  return IInventoryItem.ItemDir.Down;
        }
    }

    public override void SetImageSize(float cellSize)
    {
        _rectTransform.sizeDelta = new Vector2(cellSize * _inventoryItem.Data.Width, cellSize * _inventoryItem.Data.Height);
    }
    public override void SetPivot(IInventoryItem.ItemDir direction)
    {
        switch(direction)
        {
            case IInventoryItem.ItemDir.Down : 
                SetPivot(new Vector2(0, 1));
                break;
            case IInventoryItem.ItemDir.Right : 
                SetPivot(new Vector2(1, 1));
                break;
            case IInventoryItem.ItemDir.Up : 
                SetPivot(new Vector2(1, 0));
                break;
            case IInventoryItem.ItemDir.Left : 
                SetPivot(new Vector2(0, 0));
                break;
            default :
                SetPivot(new Vector2(0.5f, 0.5f));
                break;
        }
    }

    private void BackGroundInit()
    {
        uint occupyCellNum = _inventoryItem.Data.Width * _inventoryItem.Data.Height;
        for(int i = 0; i < occupyCellNum; i++)
        {
            Instantiate(_backGroundPrefab, _backGroundObject.transform);
        }

        _backGroundObject.SetActive(false);
    }

    public void SetAddressAndDirection(CellNumber address, IInventoryItem.ItemDir direction)
    {
        _inventoryItem.Address = address;
        _inventoryItem.Direction = direction;
    }

    public int GetRotationAngle(IInventoryItem.ItemDir itemDirection)
    {
        switch (itemDirection) 
        {
            default : return 0;
            case IInventoryItem.ItemDir.Down : return 0;
            case IInventoryItem.ItemDir .Right: return 90;
            case IInventoryItem.ItemDir .Up   : return 180;
            case IInventoryItem.ItemDir.Left : return 270;
        }
    }

    public override List<CellNumber> GetOccupyCellList(IInventoryItem.ItemDir itemDirection, CellNumber originCellNum) 
    {
        List<CellNumber> gridPositionList = new List<CellNumber>();

        switch (itemDirection)
        {
            default:
            case IInventoryItem.ItemDir.Down:
                for (int x = 0; x < _inventoryItem.Data.Width; x++) 
                {
                    for (int y = 0; y < _inventoryItem.Data.Height; y++) 
                    {
                        gridPositionList.Add(originCellNum + new CellNumber(x, y));
                    }
                }
                break;
            case IInventoryItem.ItemDir.Right:
                for (int x = 0; x < _inventoryItem.Data.Height; x++) 
                {
                    for (int y = 0; y < _inventoryItem.Data.Width; y++) 
                    {
                        gridPositionList.Add(originCellNum + new CellNumber(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }

    public override CellNumber[] GetOccupyCellArray(IInventoryItem.ItemDir itemDirection, CellNumber originCellNum)
    {
        CellNumber[] gridPositionsArray = new CellNumber[_inventoryItem.Data.Width * _inventoryItem.Data.Height];
        switch (itemDirection)
        {
            default:
            case IInventoryItem.ItemDir.Down:
                for (int x = 0; x < _inventoryItem.Data.Width; x++) 
                {
                    for (int y = 0; y < _inventoryItem.Data.Height; y++)
                    {
                        gridPositionsArray[_inventoryItem.Data.Height * x + y] = originCellNum + new CellNumber(x, y);
                    }
                }
                break;
            case IInventoryItem.ItemDir.Right:
                for (int x = 0; x < _inventoryItem.Data.Height; x++)
                {
                    for (int y = 0; y < _inventoryItem.Data.Width; y++)
                    {
                        gridPositionsArray[_inventoryItem.Data.Width * x + y] = originCellNum + new CellNumber(x, y);
                    }
                }
                break;
        }
        return gridPositionsArray;
    }

    public override void OnPointerEnter(PointerEventData pointerEventData)
    {
        //説明文表示もしたい
        if(_inventoryItem.Data.IsClickUse)_useButton.gameObject.SetActive(true);

        base.OnPointerEnter(pointerEventData);
    }

    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        if(_inventoryItem.Data.IsClickUse)_useButton.gameObject.SetActive(false);

        base.OnPointerExit(pointerEventData);
    }

    public override void OnBeginDrag(PointerEventData pointerEventData)
    {
        _useButton.gameObject.SetActive(false);
        _backGroundObject.SetActive(true);
        _canvasGroup.alpha = 0.8f;
        
        base.OnBeginDrag(pointerEventData);
    }

    public override void OnEndDrag(PointerEventData pointerEventData)
    {
        _useButton.gameObject.SetActive(false);
        _backGroundObject.SetActive(false);
        _canvasGroup.alpha = 1f;

        base.OnEndDrag(pointerEventData);
    }
}
