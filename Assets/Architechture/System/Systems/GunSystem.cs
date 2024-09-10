
using System;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : ASystem, IOnFixedUpdate
{
    private List<ABullet> bulletsList;
    private Dictionary<Enum, IObjectPool<ABullet>> bulletObjectPoolDic;

    private IObjectPool<ABullet> objectPool_10mm;

    private IBulletFactories bulletFactories;

    public override void OnSetUp()
    {
        this.objectPool_10mm = gameStat.objectPool_10mm;
        this.bulletFactories = gameStat.bullet_Factories;

        bulletsList = new List<ABullet>();

        bulletObjectPoolDic = new Dictionary<Enum, IObjectPool<ABullet>>()
        {
            
        };

        //口径ごとのObjectPoolをそれぞれSetup
        foreach (KeyValuePair<Enum, IObjectPool<ABullet>> pool in bulletObjectPoolDic)
        {
            pool.Value.PoolSetUp(bulletFactories.BulletFactory(pool.Key), 20);
        }
    }

    public void OnFixedUpdate()
    {
        foreach(ABullet bullet in bulletsList)bullet.OnFixedUpdate();
    }
}
