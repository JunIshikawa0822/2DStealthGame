using System;
using UnityEngine;

public class HandGun_CreateConcreteFactory : IFactory<HandGun>
{
    HandGun _handGun;

    public HandGun_CreateConcreteFactory(HandGun handGun)
    {
        _handGun = handGun;
    }

    public HandGun ObjectInstantiate(IObjectData data)
    {
        Scriptable_GunData handGunData = data as Scriptable_GunData;
        HandGun handGunInstance = GameObject.Instantiate(_handGun);

        return handGunInstance;
    }

    public Type GetFactoryType()
    {
        return typeof(Bullet_10mm);
    }
}
