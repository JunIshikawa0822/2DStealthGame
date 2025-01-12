using System;
using UnityEngine;

public class Gun_7_62mm_CreateConcreteFactory : IGunFactory
{
    private Handgun _handgun;
    private ObjectPool<Bullet_7_62mm> _objectPool;
    public Gun_7_62mm_CreateConcreteFactory(ObjectPool<Bullet_7_62mm> objectPool, Handgun handgun)
    {
        _objectPool = objectPool;

        _handgun = handgun;
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

        A_Item_Data baseData = data as A_Item_Data;
        newGun.OnSetUp(_objectPool);

        return newGun;
    }
}
