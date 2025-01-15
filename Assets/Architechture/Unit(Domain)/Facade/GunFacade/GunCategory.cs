using System;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
public class GunCategory : ICategory<AGun>
{
    //private int _instanceID;
    //public int InstanceID { get => InstanceID;}
    string _categoryName;
    private ICustomizeFactory _gunFactory;
    private List<AGun> _instanceList; // アイテムのリスト
    private Transform _parent;
    //ublic List<AGun> InstanceList { get => InstanceList;}

    public GunCategory(string categoryName, ICustomizeFactory gunFactory, Transform parent)
    {
        _gunFactory = gunFactory;
        _instanceList = new List<AGun>();

        _categoryName = categoryName;

        _parent = parent;
    }

    private int CheckStock(I_Data_Item data)
    {
        int index = -1;
        for(int i = _instanceList.Count - 1; i >= 0; i--)
        {
            if(_instanceList[i].Data.Equals(data))
            {
                index = i;
            }
        }

        return index;
    }

    public AGun GetInstance(I_Data_Item data)
    {
        int index = CheckStock(data);

        if(index < 0)
        {
            AGun gun = _gunFactory.ObjectInstantiate(data) as AGun;
            gun.transform.SetParent(_parent);

            return gun;
        }
        else
        {
            AGun gun = _instanceList[index];
            _instanceList.RemoveAt(index);
            
            return gun;
        }
    }

    public void Return(AGun gun)
    {
        _instanceList.Add(gun);
        gun.transform.SetParent(_parent);
    }
}
