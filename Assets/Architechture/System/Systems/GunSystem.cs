using System;
using System.Collections.Generic;

public class GunSystem : ASystem, IOnFixedUpdate
{
    //private IPlayer _player;
    List<IObjectPool> _objectPools;
    List<ICustomizeFactory> _gunFactoriesList;
    //Dictionary<IGunData.CaliberTypes, IGunFactory> _gunFactoriesDic;
    public override void OnSetUp()
    {
        //this._player = gameStat.player;
        IFactory bullet_10mm_Fac = new Bullet_10mm_CreateConcreteFactory(gameStat.bullet_10mm);
        IFactory bullet_5_56mm_Fac = new Bullet_5_56mm_CreateConcreteFactory(gameStat.bullet_5_56mm);
        IFactory bullet_7_62mm_Fac = new Bullet_7_62mm_CreateConcreteFactory(gameStat.bullet_7_62mm);
    
        ObjectPool<Bullet_10mm> bullet_10mm_Objp = new ObjectPool<Bullet_10mm>(gameStat.bulletObjectPoolTrans, bullet_10mm_Fac);
        ObjectPool<Bullet_5_56mm> bullet_5_56mm_Objp = new ObjectPool<Bullet_5_56mm>(gameStat.bulletObjectPoolTrans, bullet_5_56mm_Fac);
        ObjectPool<Bullet_7_62mm> bullet_7_62mm_Objp = new ObjectPool<Bullet_7_62mm>(gameStat.bulletObjectPoolTrans,  bullet_7_62mm_Fac);

        _objectPools = new List<IObjectPool>()
        {
            bullet_10mm_Objp,
            bullet_5_56mm_Objp,
            bullet_7_62mm_Objp
        };

        //口径ごとのObjectPoolをそれぞれSetup
        foreach(IObjectPool objectPool in _objectPools)
        {
            objectPool.PoolSetUp(20);
        }

        _gunFactoriesList = new List<ICustomizeFactory>
        {
            new HandGun_CreateConcreteFactory(gameStat.handgunPrefabs, _objectPools),
            new ShotGun_CreateConcreteFactory(gameStat.shotgunPrefabs, _objectPools)
        };

        gameStat.gunFacade = new GunFacade(_gunFactoriesList, gameStat.gunInstanceParent);
    }

    public void OnFixedUpdate()
    {
        
    }
}
