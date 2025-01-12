using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipInventory : A_Inventory
{
    private RectTransform _rectTransform;

    [SerializeField]
    private RectTransform _container;
    [SerializeField]
    private RectTransform _background;
    private IStorage _openningStorage;
    private IObjectPool _guiPool;
    private A_Item_GUI _gui_Item;

    [Range(0, 1), SerializeField, Header("1, 2どちらにアクセスするか")]
    private int _accessIndex;

    [SerializeField]
    private float _cellSize;

    private event Action<int, I_Data_Item> _onInsertEvent;
    private event Action<int, I_Data_Item> _onRemoveEvent;

    public override Action<int, I_Data_Item> InsertAction{get => _onInsertEvent; set => _onInsertEvent += value;}
    public override Action<int, I_Data_Item> RemoveAction{get => _onRemoveEvent; set => _onRemoveEvent += value;}
    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public override void Init(IObjectPool objectPool)
    {
        _guiPool = objectPool;
    }

    public override void OpenInventory(IStorage storage)
    {
        if(storage == null)
        {
            Debug.Log(this.gameObject.name + " :Storageを開けられません");
            return;
        }

        _openningStorage = storage;

        foreach(IInventoryItem item in storage.GetItems())
        {
            if(item == null)return;
            LoadItem(item);
        }
    }

    public override void CloseInventory()
    {
        if(_openningStorage == null)
        {
            Debug.Log("Storageを閉められません");
            return;
        }

        for(int i = 0; i < _container.childCount; i++)
        {
            Destroy(_container.transform.GetChild(i).gameObject);
        }

        _openningStorage = null;
    }
    
    private void LoadItem(IInventoryItem inventoryItem)
    {
        A_Item_GUI gui = _guiPool.GetFromPool() as A_Item_GUI;

        if(gui == null)return;

        gui.OnSetUp();
        gui.Init(inventoryItem);

        if(_gui_Item == null)
        {
            Vector3 newPosition = _rectTransform.position;

            gui.Item.Address = new CellNumber(0, 0);
            gui.Item.Direction = IInventoryItem.ItemDir.Down;

            InsertItem(gui, gui.Item.Address, gui.Item.Direction);

            gui.SetParent(_container);

            gui.SetScale(new Vector3(1, 1, 1));

            gui.SetPivot(IInventoryItem.ItemDir.Middle);
            gui.SetAnchorPosition(newPosition);
            gui.SetRotation(IInventoryItem.ItemDir.Middle);
            gui.SetImageSize(_cellSize);
        }
    }

    public override bool CanPlaceItem(A_Item_GUI insertGUI, CellNumber origin, IInventoryItem.ItemDir direction)
    {
        //Debug.Log("検証開始");
        bool canPlace = false;

        if(direction != IInventoryItem.ItemDir.Down) return false;
        //Debug.Log("向きは大丈夫");
        if(_gui_Item != null)return false;
        //Debug.Log("guiは大丈夫");    
        if(insertGUI.Item.Data is I_Data_Gun)
        {
            if(origin.x == 0 && origin.y == 0)
            {
                canPlace = true;
            }
        }
        return canPlace;
    }

    public override uint InsertItem(A_Item_GUI insertGUI, CellNumber origin, IInventoryItem.ItemDir direction)
    {
        _gui_Item = insertGUI;
        
        insertGUI.Item.Address = origin;
        insertGUI.Item.Direction = direction;

        insertGUI.SetParent(_container);
        insertGUI.SetPivot(IInventoryItem.ItemDir.Middle);
        insertGUI.SetPosition(_container.transform.position);
        insertGUI.SetRotation(IInventoryItem.ItemDir.Down);
        insertGUI.SetImageSize(_cellSize);

        _openningStorage.Add(insertGUI.Item);

        _onInsertEvent?.Invoke(_accessIndex, insertGUI.Item.Data);
        return 0;
    }

    public override void RemoveItem(CellNumber origin)
    {
        if(_gui_Item == null) return;
        _openningStorage.Remove(_gui_Item.Item);

        _gui_Item = null;

        _onRemoveEvent?.Invoke(_accessIndex, _gui_Item.Item.Data);
    }

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
}
