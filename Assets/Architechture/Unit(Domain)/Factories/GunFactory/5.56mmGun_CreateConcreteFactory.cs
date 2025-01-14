using System;
using UnityEngine;

public class Gun_5_56mm_CreateConcreteFactory : IGunFactory
{
    private Handgun _handgun;
    private ObjectPool<Bullet_5_56mm> _objectPool;
    public Gun_5_56mm_CreateConcreteFactory(ObjectPool<Bullet_5_56mm> objectPool, Handgun handgun)
    {
        _objectPool = objectPool;

        _handgun = handgun;
    }



    public IObject ObjectInstantiate()
    {
        return null;
    }

    public AGun GunInstantiate(IGunData data)
    {
        AGun newGun;
        if(data is Handgun_Data)
        {
            Handgun handgun = GameObject.Instantiate(_handgun);
            newGun = handgun;
        }
        else
        {
            Handgun handgun = GameObject.Instantiate(_handgun);
            newGun = handgun;
        }

        //A_Item_Data baseData = data as A_Item_Data;
        newGun.OnSetUp(_objectPool);
        //newGun.Init(d)

        return newGun;
    }
}
