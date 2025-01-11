using System;
using System.Collections;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;

public abstract class A_Inventory : MonoBehaviour
{
    public abstract Action<int, I_Data_Item> InsertAction{get;set;}
    public abstract Action<int, I_Data_Item> RemoveAction{get;set;}
    public abstract void Init(IObjectPool objectPool);
    public abstract void OpenInventory(IStorage storage);
    public abstract void CloseInventory();

    public abstract uint InsertItem(A_Item_GUI insertGUI, CellNumber origin, IInventoryItem.ItemDir direction);
    public abstract void RemoveItem(CellNumber origin);
    public abstract bool CanPlaceItem(A_Item_GUI insertGUI, CellNumber origin, IInventoryItem.ItemDir direction);

    //public abstract bool IsValid(CellNumber origin);
    public abstract bool IsCollide(A_Item_GUI gui);

    public abstract CellNumber ScreenPosToCellNum(Vector2 pos);
}
