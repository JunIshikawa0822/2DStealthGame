using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.UI;

public class EquipInventory : AInventory
{
    //public Grid<CellObject> grid;
    [SerializeField]
    private RectTransform container;
    [SerializeField]
    private RectTransform background;

    //public GUI_Item GUI_Item_Prefab;

    [Range(0, 1), Header("1, 2どちらにアクセスするか")]
    public int _accessIndex;

    private IObjectPool _objectPool;

    private IGun[] _playerGunsRef;

    private RectTransform _rectTransform;

    //public event Func<ItemData, Transform, GUI_Item> itemInstantiateEvent; 

    public float _cellSize;

    private Storage _openningStorage;

    private Vector3[] corners = new Vector3[4];

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        _rectTransform.GetWorldCorners(corners);

        for(int i = 0; i < corners.Length; i++)
        {
            Debug.Log($"{gameObject.name} : {i} : {corners[i]}");
        }
    }

    void OnSetUp(IGun[] guns)
    {
        _playerGunsRef = guns;
    }

    void Update()
    {
        //Debug.Log(gameObject.name + ":" + ScreenPToCellNum(Input.mousePosition));
        //Debug.Log(Input.mousePosition);
    }

    public override void OpenInventory(Storage storage)
    {
        //if(_playerGunsRef[_accessIndexToPlayerGuns] == null)return;
        Debug.Log(storage);

        if(storage == null)return;
        _openningStorage = storage;
        LoadItem(storage.WeaponArray[_accessIndex]);
    }

    public override void CloseInventory()
    {
        for(int i = 0; i < container.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
    }

    public override void LoadItem(ItemData data)
    {
        if(data == null)return;

        GUI_Item gui = itemInstantiateEvent?.Invoke(data, container);

        Vector3 newPosition = _rectTransform.anchoredPosition;

        data.Address = new CellNumber(0, 0);
        data.Direction = ItemData.ItemDir.Down;

        gui.SetInventory(this);
        gui.RectTransform.SetParent(container);
        gui.SetPivot(ItemData.ItemDir.Down);
        gui.SetAnchorPosition(newPosition);
        gui.SetRotation(ItemData.ItemDir.Down);
        gui.SetImageSize(_cellSize);
    }

    public override bool CanPlaceItem(GUI_Item gui, CellNumber originCellNum, ItemData.ItemDir direction)
    {
        bool canPlace = false;

        if(gui.Data.ObjectData is IGunData && direction == ItemData.ItemDir.Down)
        {
            canPlace = true;
        }

        Debug.Log(canPlace);
        return canPlace;
    }
    
    public override uint InsertItem(GUI_Item gui, CellNumber originCellNum, ItemData.ItemDir direction)
    {
        if(gui.Data.ObjectData is IGunData)
        {
            Vector3 newPosition = _rectTransform.position;

        //Dataの更新
            gui.Data.Address = new CellNumber(0, 0);
            gui.Data.Direction = ItemData.ItemDir.Down;

        //GUIの更新
            gui.SetInventory(this);
            gui.RectTransform.SetParent(container);
            gui.SetPivot(ItemData.ItemDir.Middle);
            gui.SetPosition(newPosition);
            gui.SetRotation(ItemData.ItemDir.Down);
            gui.SetImageSize(_cellSize);

            _openningStorage.AddWeapon(gui.Data, _accessIndex);
        }
        return 0;
    }

    public override void RemoveItem(CellNumber originCellNum)
    {
        _openningStorage.RemoveWeapon(_accessIndex);
    }

    public override CellNumber ScreenPosToCellNum(Vector2 pos)
    {
        CellNumber cellNum = null;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(container, pos, null, out Vector2 convertPosition);
        if(corners[0].x < pos.x && corners[0].y < pos.y)
        {
            if(corners[2].x > pos.x && corners[2].y > pos.y)
            {
                cellNum = new CellNumber(0, 0);
            }
        }

        //Debug.Log(cellNum);
        return cellNum;
    }

    public override bool IsValid(CellNumber cellNum)
    {
        bool isValid = true;
        if(cellNum == null)isValid = false;
        
        //Debug.Log(isValid);
        return isValid;
    }
}
