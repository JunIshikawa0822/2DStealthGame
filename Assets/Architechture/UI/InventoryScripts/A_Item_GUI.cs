using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_Item_GUI : A_Interactive_GUI
{
    public abstract IInventoryItem Item{get;}
    public abstract void SetRotation(IInventoryItem.ItemDir itemDir);    
    public abstract void SetPivot(IInventoryItem.ItemDir itemDir);
    public abstract void SetImageSize(float cellSize);
    public abstract void SetStackText(uint stackNum);
    public abstract (CellNumber oldAddress, IInventoryItem.ItemDir oldDir) GetOldStatus();
    public abstract void SetNewStatus(CellNumber newAddress, IInventoryItem.ItemDir newDir);
    public abstract void Init(IInventoryItem inventoryItem);
    public abstract CellNumber[] GetOccupyCellArray(CellNumber originCellNum, IInventoryItem.ItemDir itemDirection);
    public abstract List<CellNumber> GetOccupyCellList(CellNumber originCellNum, IInventoryItem.ItemDir itemDirection);
}
