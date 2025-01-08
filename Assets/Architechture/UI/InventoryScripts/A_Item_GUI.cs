using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_Item_GUI : A_Interactive_GUI
{
    public abstract void SetRotation(IInventoryItem.ItemDir itemDir);    
    public abstract void SetPivot(IInventoryItem.ItemDir itemDir);
    public abstract void SetImageSize(float cellSize);
    public abstract (CellNumber oldAddress, IInventoryItem.ItemDir oldDir) GetOldStatus();

    public abstract void SetNewStatus(CellNumber newAddress, IInventoryItem.ItemDir newDir);
    public abstract void Init(IInventoryItem inventoryItem);

    public abstract CellNumber[] GetOccupyCellArray(IInventoryItem.ItemDir itemDirection, CellNumber originCellNum);
    public abstract List<CellNumber> GetOccupyCellList(IInventoryItem.ItemDir itemDirection, CellNumber originCellNum);
}
