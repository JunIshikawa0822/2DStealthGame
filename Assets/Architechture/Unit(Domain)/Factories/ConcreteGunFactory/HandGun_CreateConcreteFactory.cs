using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandGun_CreateConcreteFactory : IGunFactory
{
    //private Handgun _handgun;
    private Handgun[] _prefabs;
    private List<IObjectPool> _objectPoolList;
    public HandGun_CreateConcreteFactory(Handgun[] prefabs, List<IObjectPool> objectPoolList)
    {
        _prefabs = prefabs;
        _objectPoolList = objectPoolList;
    }

    public IGun GunInstantiate(IGunData gunData)
    {
        Handgun gun = GameObject.Instantiate(_prefabs[0]);
        gun.HandGunInit(gunData.ShotVelocity, gunData.ShotInterval);

        IObjectPool objectPool = _objectPoolList[0];

        switch(gunData.CaliberType)
        {
            case IGunData.CaliberTypes._10mm :
                foreach(IObjectPool pool in _objectPoolList)if(objectPool is ObjectPool<Bullet_10mm>) objectPool = pool;
                break;
            
            case IGunData.CaliberTypes._5_56mm :
                foreach(IObjectPool pool in _objectPoolList)if(objectPool is ObjectPool<Bullet_5_56mm>) objectPool = pool;
                break;

            case IGunData.CaliberTypes._7_62mm : 
                foreach(IObjectPool pool in _objectPoolList)if(objectPool is ObjectPool<Bullet_7_62mm>) objectPool = pool;
                break;
        }

        A_Item_Data baseData = gunData as A_Item_Data;
        gun.OnSetUp(objectPool, baseData.ItemName);

        return gun;
    }
}
