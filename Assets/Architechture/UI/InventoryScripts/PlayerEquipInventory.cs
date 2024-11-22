using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipInventory : MonoBehaviour, IInventory
{
    //private RectTransform container;
    // [SerializeField]
    // private RectTransform background;
    private RectTransform inventoryRectTransform;
    private Vector3[] _corners;

    [SerializeField]
    private Image image;

    private Item_GUI _item;
    private IGun gun;

    void Awake()
    {
        inventoryRectTransform = this.GetComponent<RectTransform>();
        _corners = new Vector3[4];
        inventoryRectTransform.GetWorldCorners(_corners);

        _item = null;
    }

    public bool CanPlaceItem(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {
        if(item.ItemData is IGunData)return false;
        if(item.ItemDirection != Item_GUI.ItemDir.Down)return false;
        return true;
    }

    public void DecreaseItemNum(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction, uint num)
    {
        
    }


    public uint InsertItemToInventory(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {
        _item = item;

        item.SetBelongings(this, inventoryRectTransform.position, direction);
        item.RectTransform.SetParent(inventoryRectTransform);
        item.SetPivot(Item_GUI.ItemDir.Middle);
        item.SetPosition(transform.position);
        image.GetComponent<RectTransform>().position = transform.position;
        item.SetRotation(direction);
        Debug.Log("Equip!");
        return 0;
    }

    public uint InsertItemToInventory(Item_GUI item, CellNumber cellNum, Item_GUI.ItemDir direction)
    {
        _item = item;

        item.SetBelongings(this, inventoryRectTransform.position, direction);
        item.RectTransform.SetParent(inventoryRectTransform);
        item.SetPivot(Item_GUI.ItemDir.Middle);
        item.SetAnchorPosition(transform.position);
        item.SetRotation(direction);
        return 0;
    }

    public void RemoveItemFromInventory(Item_GUI item, Vector3 originPos, Item_GUI.ItemDir direction)
    {
        _item = null;
    }

    public void RemoveItemFromInventory(CellNumber cellNum)
    {
        
    }

    public bool IsValid(Vector3 pos)
    {
        float min_x = _corners[0].x;
        float max_x = _corners[2].x;
        float min_y = _corners[0].y;
        float max_y = _corners[2].y;

        //image.transform.position = new Vector3(min_x, min_y, 0);

        if(pos.x < max_x && pos.x > min_x && pos.y < max_y && pos.y > min_y)
        {
            return true;
        }

        return false;
    }

}
