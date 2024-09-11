
using System;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : ASystem, IOnFixedUpdate
{
    private List<ABullet> bulletsList;
    private IObjectPool<ABullet> bulletObjectPool;
    private IBulletFactories bulletFactories;
    private List<Type> bulletCaliberTypesList;
    private APlayer _player;

    public override void OnSetUp()
    {
        this.bulletObjectPool = gameStat.bulletObjectPool;
        this.bulletFactories = gameStat.bullet_Factories;

        this._player = gameStat.player;

        bulletCaliberTypesList = new List<Type>()
        {
            typeof(IBType_10mm),
            typeof(IBType_5_56mm),
            typeof(IBType_7_72mm)
        };

        //口径ごとのObjectPoolをそれぞれSetup
        foreach(Type IbulletType in bulletCaliberTypesList)
        {
            bulletObjectPool.PoolSetUp(bulletFactories.BulletFactory(IbulletType), 20);
        }

        _player.SetEquipment(GunObjectInstantiate(), 0);
    }

    public void OnFixedUpdate()
    {
        
    }

    public IGun GunObjectInstantiate()
    {
        IGun handgun = gameStat.Pistol1;
        handgun.OnSetUp(bulletFactories, bulletObjectPool);
        return handgun;
    }
}
