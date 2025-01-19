using System;
using System.Collections.Generic;
using UnityEngine;

public class HandGun_CreateConcreteFactory : ICustomizeFactory
{
    //private Handgun _handgun;
    private Handgun[] _prefabs;
    private List<IObjectPool> _bulletPools;
    public HandGun_CreateConcreteFactory(Handgun[] prefabs, List<IObjectPool> objectPoolList)
    {
        _prefabs = prefabs;
        _bulletPools = objectPoolList;
    }

    public IObject ObjectInstantiate(I_Data_Item data)
    {
        I_Data_HandGun gunData = data as I_Data_HandGun;
        if(gunData == null) return null;

        Handgun gun = GameObject.Instantiate(_prefabs[0]);
        IObjectPool objectPool = _bulletPools[0];

        switch(gunData.CaliberType)
        {
            case IBulletType.CaliberTypes._10mm :
                foreach(IObjectPool pool in _bulletPools)if(pool is ObjectPool<Bullet_10mm>) objectPool = pool;
                break;
            
            case IBulletType.CaliberTypes._5_56mm :
                foreach(IObjectPool pool in _bulletPools)if(pool is ObjectPool<Bullet_5_56mm>) objectPool = pool;
                break;

            case IBulletType.CaliberTypes._7_62mm :
                foreach(IObjectPool pool in _bulletPools)if(pool is ObjectPool<Bullet_7_62mm>) objectPool = pool;
                break;
        }

        //gun.GunData = gunData;
        gun.Init(gunData);
        gun.Reload(new Entity_Magazine(gunData.MaxAmmoNum, gunData.MaxAmmoNum));
        gun.OnSetUp(objectPool);

        return gun;
    }
}
