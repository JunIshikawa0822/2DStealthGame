using System;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.XR;

public class Gun_10mm_CreateConcreteFactory : IGunFactory
{
    private Handgun _handgun;
    private ObjectPool<Bullet_10mm> _objectPool;
    public Gun_10mm_CreateConcreteFactory(ObjectPool<Bullet_10mm> objectPool, Handgun handgun)
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

            handgun.Reload(new Entity_Magazine(data.MaxAmmoNum, 0));
            newGun = handgun;
        }
        else
        {
            Handgun handgun = GameObject.Instantiate(_handgun);

            handgun.Reload(new Entity_Magazine(data.MaxAmmoNum, 0));
            newGun = handgun;
        }

        A_Item_Data baseData = data as A_Item_Data;
        newGun.OnSetUp(_objectPool);
        return newGun;
    }
}
