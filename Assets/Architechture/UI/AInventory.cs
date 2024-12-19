using UnityEngine;
using System;

public abstract class AInventory : MonoBehaviour
{
    public Func<ItemData, Transform, GUI_Item> itemInstantiateEvent; 

    public Action<int, ItemData> onInsertEvent;
    public Action<int> onRemoveEvent;

    public abstract void OnSetUp(ItemFacade facade);
    public abstract void OpenInventory(Storage storage);
    public abstract void CloseInventory();
    public abstract CellNumber ScreenPosToCellNum(Vector2 pos);
    public abstract bool CanPlaceItem(GUI_Item gui, CellNumber originCellNum, ItemData.ItemDir direction);
    public abstract uint InsertItem(GUI_Item gui, CellNumber originCellNum, ItemData.ItemDir direction);
    public abstract void RemoveItem(CellNumber originCellNum);
    public abstract bool IsValid(CellNumber originCellNum);
    public abstract void LoadItem(ItemData data);
}
