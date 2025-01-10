using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipInventory : A_Inventory
{
    private RectTransform _rectTransform;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public override void Init(IObjectPool objectPool)
    {
        
    }

    public override void OpenInventory(IStorage storage)
    {

    }

    public override void CloseInventory()
    {
        
    }

    public override bool CanPlaceItem(A_Item_GUI insertGUI, CellNumber origin, IInventoryItem.ItemDir direction)
    {
        return true;
    }

    public override uint InsertItem(A_Item_GUI insertGUI, CellNumber origin, IInventoryItem.ItemDir direction)
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveItem(CellNumber origin)
    {
        throw new System.NotImplementedException();
    }

    // public override bool IsValid(CellNumber origin)
    // {
    //     return true;
    // }

    public override bool IsCollide(A_Item_GUI gui)
    {
        Vector3[] inventoryRect = new Vector3[4];
        _rectTransform.GetWorldCorners(inventoryRect);

        Vector3[] guiRect = new Vector3[4];
        gui.RectTransform.GetWorldCorners(guiRect);

        //重なっていない
        if(guiRect[0].x >= inventoryRect[2].x 
        || guiRect[2].x <= inventoryRect[0].x 
        || guiRect[0].y >= inventoryRect[2].y 
        || guiRect[2].y <= inventoryRect[0].y) return false;

        float threshold = 0.4f;

        //重なっているとき
        float overlapX1 = Mathf.Max(guiRect[0].x, inventoryRect[0].x);
        float overlapY1 = Mathf.Max(guiRect[0].y, inventoryRect[0].y);

        float overlapX2 = Mathf.Max(guiRect[2].x, inventoryRect[2].x);
        float overlapY2 = Mathf.Max(guiRect[2].y, inventoryRect[2].y);

        float overlapWidth = overlapX1 * overlapX2;
        float overlapHeight = overlapY1 * overlapY2;

        float overlapArea = Mathf.Max(0, overlapWidth) * Mathf.Max(0, overlapHeight);
        float guiArea = (guiRect[2].x - guiRect[0].x) * (guiRect[2].y - guiRect[0].y);

        if(overlapArea < guiArea * threshold) return false;

        return true;
    }

    public override CellNumber ScreenPosToCellNum(Vector2 pos)
    {
        return new CellNumber(0, 0);
    }

    public override Vector3[] GetCorners()
    {
        Vector3[] corners = new Vector3[4];
        _rectTransform.GetWorldCorners(corners);

        return corners;
    }
}
