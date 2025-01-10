
using UnityEngine;
using System;

public class Item_GUI_CreateConcreteFactory : IFactory
{
    Item_GUI _item_GUI;

    Action<A_Item_GUI> onPointerDownAction;
    Action<A_Item_GUI> onPointerUpAction;

    public Item_GUI_CreateConcreteFactory(Item_GUI itemGUI, Action<A_Item_GUI> pDown, Action<A_Item_GUI> pUp)
    {
        _item_GUI = itemGUI;

        onPointerDownAction += pDown;
        onPointerUpAction += pUp;
    }

    public IObject ObjectInstantiate()
    {
        Item_GUI newGUI = GameObject.Instantiate(_item_GUI);

        newGUI.onPointerDownEvent += onPointerDownAction;
        newGUI.onPointerUpEvent += onPointerUpAction;

        return newGUI;
    }

    public IObject ObjectInstantiate(A_Item_Data data)
    {
        return GameObject.Instantiate(_item_GUI);;
    }

    public IObject ObjectInstantiate(I_Data_Item data)
    {
        return GameObject.Instantiate(_item_GUI);
    }
}
