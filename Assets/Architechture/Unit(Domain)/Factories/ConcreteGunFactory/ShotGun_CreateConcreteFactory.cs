using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ShotGun_CreateConcreteFactory : IGunFactory
{
    //private Handgun _handgun;
    private Shotgun[] _prefabs;
    private List<IObjectPool> _bulletPools;
    public ShotGun_CreateConcreteFactory(Shotgun[] prefabs, List<IObjectPool> objectPoolList)
    {
        _prefabs = prefabs;
        _bulletPools = objectPoolList;
    }

    public AGun GunInstantiate(IGunData gunData)
    {
        Shotgun gun = GameObject.Instantiate(_prefabs[0]);

        Shotgun_Data shotgun_Data = gunData as Shotgun_Data;

        gun.ShotgunInit(
            shotgun_Data.ShotVelocity, 
            shotgun_Data.ShotInterval,
            shotgun_Data.simulNum,
            shotgun_Data.spreadAngle);

        IObjectPool objectPool = _bulletPools[0];

        switch(gunData.CaliberType)
        {
            case IGunData.CaliberTypes._10mm :
                foreach(IObjectPool pool in _bulletPools)if(pool is ObjectPool<Bullet_10mm>) objectPool = pool;
                break;
            
            case IGunData.CaliberTypes._5_56mm :
                foreach(IObjectPool pool in _bulletPools)if(pool is ObjectPool<Bullet_5_56mm>) objectPool = pool;
                break;

            case IGunData.CaliberTypes._7_62mm : 
                foreach(IObjectPool pool in _bulletPools)if(pool is ObjectPool<Bullet_7_62mm>) objectPool = pool;
                break;
        }

        gun.GunData = gunData;
        gun.Reload(new Entity_Magazine(gunData.MaxAmmoNum, 0));

        A_Item_Data baseData = gunData as A_Item_Data;
        gun.OnSetUp(objectPool);

        return gun;
    }
}
