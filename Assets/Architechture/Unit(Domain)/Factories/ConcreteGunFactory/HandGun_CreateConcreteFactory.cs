using System;
using System.Collections.Generic;
using UnityEngine;

public class HandGun_CreateConcreteFactory : IGunFactory
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
        I_Data_Gun gunData = null;

        if(data is A_Data_Fixed_Gun fixedGunData)
        {
            gunData = fixedGunData;
        }
        else if(data is A_Data_Customizable_Gun customizableGunData)
        {
            gunData = customizableGunData;
        }

        if(gunData == null) return null;

        Handgun gun = GameObject.Instantiate(_prefabs[0]);
        IObjectPool objectPool = _bulletPools[0];

        switch(gunData.CaliberType)
        {
            case I_Data_Gun.CaliberTypes._10mm :
                foreach(IObjectPool pool in _bulletPools)if(pool is ObjectPool<Bullet_10mm>) objectPool = pool;
                break;
            
            case I_Data_Gun.CaliberTypes._5_56mm :
                foreach(IObjectPool pool in _bulletPools)if(pool is ObjectPool<Bullet_5_56mm>) objectPool = pool;
                break;

            case I_Data_Gun.CaliberTypes._7_62mm :
                foreach(IObjectPool pool in _bulletPools)if(pool is ObjectPool<Bullet_7_62mm>) objectPool = pool;
                break;
        }

        //gun.GunData = gunData;
        gun.HandGunInit(gunData.ShotVelocity, gunData.ShotInterval);
        gun.Reload(new Entity_Magazine(gunData.MaxAmmoNum, 0));
        gun.OnSetUp(objectPool);

        return gun;
    }

    public AGun GunInstantiate(IGunData gunData)
    {
        Handgun gun = GameObject.Instantiate(_prefabs[0]);
        gun.HandGunInit(gunData.ShotVelocity, gunData.ShotInterval);

        IObjectPool objectPool = _bulletPools[0];

        // Debug.Log(string.Join(", " , _objectPoolList));

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
