using System;
using System.Collections.Generic;

public class GunSystem : ASystem, IOnFixedUpdate
{
    private IPlayer _player;
    IFactory _bullet_10mm_Fac;
    IFactory _bullet_5_56mm_Fac;
    IFactory _bullet_7_62mm_Fac;

    List<IObjectPool> objectPools;

    public override void OnSetUp()
    {
        //this._player = gameStat.player;
        IFactory bullet_10mm_Fac = new Bullet_10mm_CreateConcreteFactory(gameStat.bullet_10mm);
        IFactory bullet_5_56mm_Fac = new Bullet_5_56mm_CreateConcreteFactory(gameStat.bullet_5_56mm);
        IFactory bullet_7_62mm_Fac = new Bullet_7_62mm_CreateConcreteFactory(gameStat.bullet_7_62mm);
    
        ObjectPool<Bullet_10mm> bullet_10mm_Objp = new ObjectPool<Bullet_10mm>(gameStat.bulletObjectPoolTrans, bullet_10mm_Fac);
        ObjectPool<Bullet_5_56mm> bullet_5_56mm_Objp = new ObjectPool<Bullet_5_56mm>(gameStat.bulletObjectPoolTrans, bullet_5_56mm_Fac);
        ObjectPool<Bullet_7_62mm> bullet_7_72mm_Objp = new ObjectPool<Bullet_7_62mm>(gameStat.bulletObjectPoolTrans,  bullet_7_62mm_Fac);

        objectPools = new List<IObjectPool>()
        {
            bullet_10mm_Objp,
            bullet_5_56mm_Objp,
            bullet_7_72mm_Objp
        };
        
        IFactory gun_10mm_Fac = new Gun_10mm_CreateConcreteFactory(bullet_10mm_Objp, gameStat.Pistol1);
        //IFactory gun_5_56mm_Fac = new Gun_5_56mm_CreateConcreteFactory(bullet_5_56mm_objp, )

        gameStat.gunFactoriesDictionary.Add(IGunData.CaliberTypes._10mm, gun_10mm_Fac);

        //口径ごとのObjectPoolをそれぞれSetup
        foreach(IObjectPool objectPool in objectPools)
        {
            objectPool.PoolSetUp(20);
        }
    }

    public void OnFixedUpdate()
    {
        
    }
}
