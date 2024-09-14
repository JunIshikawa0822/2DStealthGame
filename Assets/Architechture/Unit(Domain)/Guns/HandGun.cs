using UnityEngine;
using System;

public class HandGun : MonoBehaviour, IGun_10mm
{
    [SerializeField]
    private float _muzzleVelocity = 700f;
    [SerializeField] 
    private Transform _muzzlePosition;
    //もっと一般化していい奴ら↓
    private IObjectPool<ABullet> _objectPool;
    private IBulletFactories _bulletFactories;
    private IFactory<ABullet> _bulletcaliberFactory;

    public void OnSetUp(IBulletFactories bulletFactories, IObjectPool<ABullet> objectPool)
    {
        this._bulletFactories = bulletFactories;
        this._objectPool = objectPool;
        
        _bulletcaliberFactory = GetFactory_10mm<IBType_10mm>();
    }

    public void OnUpdate()
    {

    }

    public IFactory<ABullet> GetFactory_10mm<T>() where T : IBType_10mm
    {
        // ここで、T 型が IBType_10mm を継承していることが保証されています
        Type factoryType = typeof(T);
        return _bulletFactories.BulletFactory(factoryType);
    }

    public void Shot()
    {
        _bulletcaliberFactory = GetFactory_10mm<IBType_10mm>();
        if(_bulletcaliberFactory == null)return;
        
        ABullet bullet = _objectPool.GetFromPool(_bulletcaliberFactory) as ABullet;

        if(bullet == null)
        {
            Debug.Log("キャスト無理ぃ");
            return;
        }

        if (bullet.gameObject == null)return;

        bullet.Init(_muzzlePosition.position);
        bullet.GetBulletTransform().SetPositionAndRotation(_muzzlePosition.position, _muzzlePosition.rotation);
        bullet.GetBulletRigidbody().AddForce(bullet.gameObject.transform.forward * _muzzleVelocity, ForceMode.Acceleration);
    }

    public void Reload()
    {

    }

    public void Jam()
    {

    }
}
