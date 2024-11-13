using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipInventory : MonoBehaviour
{
    private RectTransform inventoryRectTransform;
    private Item_GUI _item;

    void Awake()
    {
        inventoryRectTransform = this.GetComponent<RectTransform>();
    }

    public bool CanPlaceItem(Item_GUI item)
    {
        if(item.GetItemData().itemKind != Scriptable_ItemData.ItemKind.Gun)return false;

        return true;
    }

    public uint InsertItemToInventory(Item_GUI item)
    {
        //item.SetBelongings(this, originCellNum, Item_GUI.ItemDir.Down, inventoryRectTransform);
        item.SetAnchor(Item_GUI.ItemDir.Down);
        item.SetAnchorPosition(this.transform.position);
        item.SetRotation(Item_GUI.ItemDir.Down);

        return 0;
    }

    public void RemoveItemFromInventory(Item_GUI item, CellNumber originCellNum, Item_GUI.ItemDir direction)
    {

    }
}
