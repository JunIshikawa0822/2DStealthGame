using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class A_Item_GUI : A_Interactive_GUI
{
    public event Action<A_Item_GUI> onPointerDownEvent;
    public event Action<A_Item_GUI> onPointerUpEvent;

    public abstract IInventoryItem Item{get;}
    //public abstract A_Inventory BelongingInventory{get;}

    public abstract void SetRotation(IInventoryItem.ItemDir itemDir);    
    public abstract void SetPivot(IInventoryItem.ItemDir itemDir);
    public abstract void SetImageSize(float cellSize);
    public abstract void SetStackText(uint stackNum);
    public abstract (CellNumber oldAddress, IInventoryItem.ItemDir oldDir) GetOldStatus();
    public abstract void SetNewStatus(CellNumber newAddress, IInventoryItem.ItemDir newDir);
    public abstract void Init(IInventoryItem inventoryItem);
    public abstract CellNumber[] GetOccupyCellArray(CellNumber originCellNum, IInventoryItem.ItemDir itemDirection);
    public abstract List<CellNumber> GetOccupyCellList(CellNumber originCellNum, IInventoryItem.ItemDir itemDirection);

    //計算

    public abstract IInventoryItem.ItemDir GetNextDir(IInventoryItem.ItemDir direction);
    public abstract float GetRotationAngle(IInventoryItem.ItemDir direction);

    public override void OnPointerDown(PointerEventData pointerEventData)
    {
        onPointerDownEvent?.Invoke(this);
    }

    public override void OnPointerUp(PointerEventData pointerEventData)
    {
        onPointerUpEvent?.Invoke(this);
    }
}
