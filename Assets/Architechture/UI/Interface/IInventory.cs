using UnityEngine;

public interface IInventory
{
    public CellNumber ScreenPosToCellNum(Vector2 pos);
    public bool CanPlaceItem(GUI_Item gui, CellNumber originCellNum, ItemData.ItemDir direction);
    public uint InsertItem(GUI_Item gui, CellNumber originCellNum, ItemData.ItemDir direction);
    public void RemoveItem(CellNumber originCellNum);
    public bool IsValid(CellNumber originCellNum);
    public void LoadItem(ItemData data);

}
