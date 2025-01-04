using System;
using System.Collections.Generic;
using UnityEngine;
public class GunCategory
{
    //private int _instanceID;
    //public int InstanceID { get => InstanceID;}
    string _categoryName;
    private IGunFactory _gunFactory;
    private List<AGun> _instanceList; // アイテムのリスト
    private Transform _parent;
    //ublic List<AGun> InstanceList { get => InstanceList;}

    public GunCategory(string categoryName, IGunFactory gunFactory, Transform parent)
    {
        _gunFactory = gunFactory;
        _instanceList = new List<AGun>();

        _categoryName = categoryName;

        _parent = parent;
    }

    private int FindInstanceIndex(IGunData data)
    {
        int index = -1;

        for(int i = _instanceList.Count - 1; i >= 0; i--)
        {
            if(_instanceList[i].GunData.Equals(data))
            {
                index = i;
                break;
            }
        }

        return index;
    }

    public AGun GetInstance(IGunData data)
    {
        int index = FindInstanceIndex(data);
        // Debug.Log(index);

        if(index < 0)
        {
            AGun gun = _gunFactory.GunInstantiate(data);
            gun.transform.SetParent(_parent);

            return gun;
        }
        else
        {
            return _instanceList[index];
        }
    }

    public void ReturnToList(AGun gun)
    {
        _instanceList.Add(gun);
        gun.transform.SetParent(_parent);
    }
}
