using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities.UniversalDelegates;

public class ItemFacade
{
    //オブジェクトとして使用しないもの（インスタンスが唯一であるアイテムだけ）
    Dictionary<int, IItem> _itemInstaceDic;
    public ItemFacade(Dictionary<int, IItem> itemInstaceDic)
    {
        _itemInstaceDic = itemInstaceDic;
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
        UnityEngine.Debug.Log(string.Join(",", _itemInstaceDic));
    }
}
