using UnityEngine;

public interface IInventory
{
    public bool CanPlaceItem(Item_GUI item);
    public uint InsertItemToInventory(Item_GUI item, CellNumber originCellNum, Item_GUI.ItemDir direction);
    public void RemoveItemFromInventory(Item_GUI item, CellNumber originCellNum, Item_GUI.ItemDir direction);
}
