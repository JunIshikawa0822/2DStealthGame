using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities.UniversalDelegates;

public class ItemFacade
{
    Dictionary<int, IItem> _itemInstaceDic;
    public ItemFacade(Dictionary<int, IItem> itemInstaceDic)
    {
        _itemInstaceDic = itemInstaceDic;

        foreach(KeyValuePair<int, IItem> item in _itemInstaceDic)
        {
            if(item.Value is IGun) 
            {
                IGun gun = item.Value as IGun;
                gun.ObjectActive(false);
            }
        }
    }

    public void ItemUse()
    {
        
    }

    public IItem GetItemObject(int ID)
    {
        return _itemInstaceDic[ID];
    }

    public void CheckRef(int ID)
    {
        UnityEngine.Debug.Log(_itemInstaceDic[ID].Name);
    }
}
