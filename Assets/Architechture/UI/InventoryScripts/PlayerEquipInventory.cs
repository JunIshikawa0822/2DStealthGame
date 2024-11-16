using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipInventory : MonoBehaviour, IInventory
{
    private RectTransform inventoryRectTransform;
    private Item_GUI _item;

    void Awake()
    {
        inventoryRectTransform = this.GetComponent<RectTransform>();
    }

    public bool CanPlaceItem(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {
        return true;
    }


    public uint InsertItemToInventory(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {
        return 0;
    }

    public void RemoveItemFromInventory(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {

    }

    public bool IsValid(Vector3 pos)
    {
        return true;
    }

}
