
using System;
using System.Collections.Generic;

public class GunSystem : ASystem, IOnFixedUpdate
{
    //private List<ABullet> bulletsList;
    private IObjectPool<ABullet> _bulletObjectPool;
    private IBulletFactories _bulletFactories;

    private Dictionary<Type, IFactory<ABullet>> _factoriesDic;
    //private List<Type> _bulletCaliberTypesList;
    private APlayer _player;

    public override void OnSetUp()
    {
        this._player = gameStat.player; 
        this._bulletObjectPool = new BulletObjectPool(gameStat.bulletObjectPoolTrans);

        _factoriesDic = new Dictionary<Type, IFactory<ABullet>>
        {
            {typeof(IBType_10mm), new Bullet_10mm_CreateConcreteFactory(gameStat.bullet_10mm)},
            {typeof(IBType_5_56mm), new Bullet_5_56mm_CreateConcreteFactory(gameStat.bullet_5_56mm)},
            {typeof(IBType_7_72mm), new Bullet_7_62mm_CreateConcreteFactory(gameStat.bullet_7_62mm)}
        };

        this._bulletFactories = new Bullet_Factories(_factoriesDic);

        //口径ごとのObjectPoolをそれぞれSetup
        _bulletObjectPool.PoolSetUp(_bulletFactories.BulletFactory(typeof(IBType_10mm)), 20);
        _bulletObjectPool.PoolSetUp(_bulletFactories.BulletFactory(typeof(IBType_5_56mm)), 20);
        _bulletObjectPool.PoolSetUp(_bulletFactories.BulletFactory(typeof(IBType_7_72mm)), 20);
        
        _player.SetEquipment(GunObjectInstantiate(), 0);
    }

    public void OnFixedUpdate()
    {
        
    }

    public IGun GunObjectInstantiate()
    {
        IGun handgun = gameStat.Pistol1;
        handgun.OnSetUp(_bulletFactories, _bulletObjectPool);
        return handgun;
    }
}
