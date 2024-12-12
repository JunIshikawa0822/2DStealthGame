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

        // foreach(KeyValuePair<int, IItem> item in _itemInstaceDic)
        // {
        //     if(item.Value is IGun) 
        //     {
        //         MonoBehaviour gunOject = item.Value as MonoBehaviour;
        //         gunOject.gameObject.SetActive(false);
        //     }
        // }
    }

    public void ItemUse()
    {
        
    }

    public void CheckRef(int ID)
    {
        UnityEngine.Debug.Log(_itemInstaceDic[ID].Name);
    }
}
