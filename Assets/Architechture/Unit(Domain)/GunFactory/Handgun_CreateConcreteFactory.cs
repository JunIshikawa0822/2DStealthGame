using System;
using UnityEngine;

public class HandGun_CreateConcreteFactory : IFactory<HandGun>
{
    HandGun _handGun;

    public HandGun_CreateConcreteFactory(HandGun handGun)
    {
        _handGun = handGun;
    }

    public HandGun ObjectInstantiate(IObjectData handGunData)
    {
        HandGun handGunInstance = GameObject.Instantiate(_handGun);

        return handGunInstance;
    }

    public Type GetFactoryType()
    {
        return typeof(Bullet_10mm);
    }
}
