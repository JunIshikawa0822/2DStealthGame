using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities.UniversalDelegates;

public class ItemFacade
{
    //オブジェクトとして使用しないもの（インスタンスが唯一であるアイテムだけ）
    Dictionary<int, IObject> _itemInstaceDic;
    public ItemFacade(Dictionary<int, IObject> itemInstaceDic)
    {
        _itemInstaceDic = itemInstaceDic;
    }

    public void ItemUse()
    {
        
    }

    public IObject GetItemObject(int ID)
    {
        return _itemInstaceDic[ID];
    }

    public void CheckRef(int ID)
    {
        //UnityEngine.Debug.Log(_itemInstaceDic[ID].Name);
        UnityEngine.Debug.Log(string.Join(",", _itemInstaceDic));
    }
}
