using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Rifle_CreateConcreteFactory : ICustomizeFactory
{
    //private Handgun _handgun;
    private Rifle[] _prefabs;
    private List<IObjectPool> _bulletPools;
    public Rifle_CreateConcreteFactory(Rifle[] prefabs, List<IObjectPool> objectPoolList)
    {
        _prefabs = prefabs;
        _bulletPools = objectPoolList;
    }

        public IObject ObjectInstantiate(I_Data_Item data)
    {
        I_Data_Rifle gunData = data as I_Data_Rifle;
        if(gunData == null) return null;

        Rifle gun = GameObject.Instantiate(_prefabs[0]);
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

        gun.OnSetUp(objectPool);
        gun.Init(gunData);
        //gun.GunData = gunData;
        gun.Reload(new Entity_Magazine(gunData.MaxAmmoNum, gunData.MaxAmmoNum));

        return gun;
    }

}
