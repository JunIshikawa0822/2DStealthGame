using System;
using System.Collections.Generic;

public class GunSystem : ASystem, IOnFixedUpdate
{
    //private List<ABullet> bulletsList;
    //private IObjectPool<Bullet_10mm> _bullet_10mm_ObjectPool;
    //private IFactory<Bullet_10mm> _bullet_10mm_factory;
    //private IBulletFactories _bulletFactories;
    //private Dictionary<Type, IFactory<ABullet>> _factoriesDic;
    //private List<Type> _bulletCaliberTypesList;
    private IPlayer _player;

    public override void OnSetUp()
    {
        this._player = gameStat.player;

        
        gameStat.bullet_10mm_factory = new Bullet_10mm_CreateConcreteFactory(gameStat.bullet_10mm);
        gameStat.bullet_10mm_ObjectPool = new ObjectPool<Bullet_10mm>(gameStat.bulletObjectPoolTrans, gameStat.bullet_10mm_factory);
        //口径ごとのObjectPoolをそれぞれSetup
        gameStat.bullet_10mm_ObjectPool.PoolSetUp(20);

        // gameStat.playerGunsArray[0] = GunObjectInstantiate();
        //_player.SetEquipment(GunObjectInstantiate(), 0);
    }

    public void OnFixedUpdate()
    {
        
    }

#region やっつけ
    public IGun<Bullet_10mm> GunObjectInstantiate()
    {
        IGun<Bullet_10mm> gun = gameStat.Pistol1;
        gun.Reload(new Entity_Magazine(10, 10));

        gun.OnSetUp(gameStat.bullet_10mm_ObjectPool);
        
        return gun;
    }
#endregion
}
