using System;
using UnityEngine;

public class Shotgun_CreateConcreteFactory : IFactory<Handgun>
{
    Handgun _handGun;

    public Shotgun_CreateConcreteFactory(Handgun handGun)
    {
        _handGun = handGun;
    }

    public Handgun ObjectInstantiate(IObjectData data)
    {
        Scriptable_Handgun_Data handGunData = data as Scriptable_Handgun_Data;

        Handgun handGunInstance = GameObject.Instantiate(_handGun);
        handGunInstance.HandGunInit(handGunData.muzzleVelocity, handGunData.shotInterval);

        return handGunInstance;
    }

    public Type GetFactoryType()
    {
        return typeof(Handgun);
    }
}
