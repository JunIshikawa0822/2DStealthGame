using UnityEngine;

public interface IInventory
{
    public bool CanPlaceItem(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction);
    public uint InsertItemToInventory(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction);
    public uint InsertItemToInventory(Item_GUI item, CellNumber cellNum, Item_GUI.ItemDir direction);
    public void RemoveItemFromInventory(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction);

    public void RemoveItemFromInventory(CellNumber cellNum);

    public void DecreaseItemNum(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction, uint num);

    public bool IsValid(Vector3 pos);
}
