using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipInventory : MonoBehaviour, IInventory
{
    //private RectTransform container;
    // [SerializeField]
    // private RectTransform background;
    private RectTransform inventoryRectTransform;
    private Vector3[] _corners;

    private Item_GUI _item;

    void Awake()
    {
        inventoryRectTransform = this.GetComponent<RectTransform>();
        _corners = new Vector3[4];
        inventoryRectTransform.GetWorldCorners(_corners);
    }

    public bool CanPlaceItem(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {
        if(item.GetItemData().itemKind != Scriptable_ItemData.ItemKind.Gun)return false;
        if(item.GetDirection() != Item_GUI.ItemDir.Down)return false;
        return true;
    }


    public uint InsertItemToInventory(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {
        item.SetBelongings(this, inventoryRectTransform.position, direction);
        item.GetRectTransform().SetParent(inventoryRectTransform);
        item.SetAnchor(direction);
        item.SetAnchorPosition(inventoryRectTransform.position);
        item.SetRotation(direction);
        return 0;
    }

    public uint InsertItemToInventory(Item_GUI item, CellNumber cellNum, Item_GUI.ItemDir direction)
    {
        item.SetBelongings(this, inventoryRectTransform.position, direction);
        item.GetRectTransform().SetParent(inventoryRectTransform);
        item.SetAnchor(direction);
        item.SetAnchorPosition(inventoryRectTransform.position);
        item.SetRotation(direction);
        return 0;
    }

    public void RemoveItemFromInventory(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {
        _item = null;
    }

    public bool IsValid(Vector3 pos)
    {
        float min_x = _corners[0].x;
        float max_x = _corners[2].x;
        float min_y = _corners[0].y;
        float max_y = _corners[2].y;

        if(pos.x < max_x && pos.x > min_x && pos.y < max_y && pos.y > min_y)
        {
            return true;
        }

        return false;
    }

}
