using System;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.XR;

public class Gun_10mm_CreateConcreteFactory : IFactory
{
    private Handgun _handgun;
    private ObjectPool<Bullet_10mm> _objectPool;
    public Gun_10mm_CreateConcreteFactory(ObjectPool<Bullet_10mm> objectPool, Handgun handgun)
    {
        _objectPool = objectPool;

        _handgun = handgun;
    }

    public IGun GunInstantiate(IGunData data)
    {
        IGun newGun;
        if(data is Handgun_Data)
        {
            Handgun handgun = GameObject.Instantiate(_handgun);
            handgun.HandGunInit(data.ShotVelocity, data.ShotInterval);
            newGun = handgun;
        }
        else
        {
            Handgun handgun = GameObject.Instantiate(_handgun);
            handgun.HandGunInit(700, 0.5f);
            newGun = handgun;
        }

        newGun.OnSetUp(_objectPool);

        return newGun;
    }

    public IItem ObjectInstantiate()
    {
        return null;
    }

    public IItem ObjectInstantiate(A_Item_Data data)
    {
        IGunData gunData = data as IGunData;
        
        IGun newGun;
        if(data is Handgun_Data)
        {
            Handgun handgun = GameObject.Instantiate(_handgun);
            handgun.HandGunInit(gunData.ShotVelocity, gunData.ShotInterval);
            newGun = handgun;
        }
        else
        {
            Handgun handgun = GameObject.Instantiate(_handgun);
            handgun.HandGunInit(700, 0.5f);
            newGun = handgun;
        }

        newGun.OnSetUp(_objectPool);

        return newGun as IItem;
    }
}
