using System;
using System.Collections.Generic;
public class GunCategory
{
    //private int _instanceID;
    //public int InstanceID { get => InstanceID;}
    string _categoryName;
    private IGunFactory _gunFactory;
    private List<AGun> _instanceList; // アイテムのリスト
    //ublic List<AGun> InstanceList { get => InstanceList;}

    public GunCategory(string CategoryName, IGunFactory gunFactory)
    {
        _gunFactory = gunFactory;
        _instanceList = new List<AGun>();

        _categoryName = CategoryName;
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

        if(index < 0)
        {
            AGun gun = _gunFactory.GunInstantiate(data);
            _instanceList.Add(gun);

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
    }
}
