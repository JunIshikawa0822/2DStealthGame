using System;
using UnityEngine;

public class SubMachinegun_CreateConcreteFactory : IGunFactory
{
    GameObject _submachinegun;

    public SubMachinegun_CreateConcreteFactory(GameObject submachinegun)
    {
        _submachinegun = submachinegun;
    }

    public IGun GunInstantiate(IGunData data)
    {
        if(data == null)return null;

        Shotgun_Data gunData = data as Shotgun_Data;
        if(gunData == null) return null;

        Shotgun<Bullet_10mm> instance = GameObject.Instantiate(_submachinegun).GetComponent<Shotgun<Bullet_10mm>>();
        instance.ShotgunInit(gunData.simulNum, gunData.ShotVelocity, gunData.ShotInterval);
        return instance;
    }
}
