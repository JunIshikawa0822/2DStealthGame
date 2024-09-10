using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
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

        GameObject bulletObject = _objectPool.GetFromPool(_bulletcaliberFactory).gameObject;
        if (bulletObject == null)return;

        bulletObject.transform.SetPositionAndRotation(_muzzlePosition.position, _muzzlePosition.rotation);
        bulletObject.GetComponent<Rigidbody>().AddForce(bulletObject.transform.forward * _muzzleVelocity, ForceMode.Acceleration);
    }

    public void Reload()
    {

    }

    public void Jam()
    {

    }
}
