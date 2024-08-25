using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacedObject : ItemDragAndDrop<PlacedObject>
{
    private Scriptable_UI_Item itemData;

    //[System.NonSerialized]
    public Scriptable_UI_Item.Dir direction;

    [System.NonSerialized]
    public RectTransform rectTransform;

    [System.NonSerialized]
    public PlacedObject placedObject;

    [System.NonSerialized]
    public TetrisInventory belongingInventory;

    [System.NonSerialized]
    public Vector2Int belongingCellNum;

    public void OnSetUp(Scriptable_UI_Item itemData)
    {
        rectTransform = GetComponent<RectTransform>();
        this.itemData = itemData;
        this.direction = itemData.direction;
        Tobject = this;
    }

    public void ImageSizeSet(float width, float height)
    {
        rectTransform.sizeDelta = new Vector2(width, height);
    }

    public Scriptable_UI_Item GetItemData()
    {
        return itemData;
    }
}
