using System;
using System.Collections.Generic;

public class GunSystem : ASystem, IOnFixedUpdate
{
    //private List<ABullet> bulletsList;
    private IObjectPool<Bullet_10mm> _bullet_10mm_ObjectPool;
    private IFactory<Bullet_10mm> _bullet_10mm_factory;
    //private IBulletFactories _bulletFactories;

    //private Dictionary<Type, IFactory<ABullet>> _factoriesDic;
    //private List<Type> _bulletCaliberTypesList;
    private APlayer _player;

    public override void OnSetUp()
    {
        this._player = gameStat.player;

        _bullet_10mm_ObjectPool = new ObjectPool<Bullet_10mm>(gameStat.bulletObjectPoolTrans);
        _bullet_10mm_factory = new Bullet_10mm_CreateConcreteFactory(gameStat.bullet_10mm);

        //口径ごとのObjectPoolをそれぞれSetup
        _bullet_10mm_ObjectPool.PoolSetUp(_bullet_10mm_factory, 20);

        gameStat.playerGunsArray[0] = GunObjectInstantiate();
        //_player.SetEquipment(GunObjectInstantiate(), 0);
    }

    public void OnFixedUpdate()
    {
        
    }

    public IGun GunObjectInstantiate()
    {
        IGun gun = gameStat.Pistol1;
        gun.OnSetUp(_bullet_10mm_factory, _bullet_10mm_ObjectPool);
        gun.Reload(new Entity_Magazine(10, 10));
        return gun;
    }
}
