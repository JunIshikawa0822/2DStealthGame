using UnityEngine;
using System;

public abstract class  ABullet : APooledObject<ABullet>, IOnFixedUpdate
{
    protected int _bulletDamage;
    protected Rigidbody _bulletRigidbody;
    protected Transform _bulletTransform;
    // protected APooledObject<ABullet> _pooledObject;

    public Rigidbody GetObjectRigidbody(){return _bulletRigidbody;}
    public Transform GetObjectTransform(){return _bulletTransform;}

    //public APooledObject<ABullet> GetPooledObject(){return _pooledObject;}

    public void OnSetUp()
    {
        //this._bulletDamage = bulletDamage;
        _bulletRigidbody = GetComponent<Rigidbody>();
        _bulletTransform = GetComponent<Transform>();
        //_pooledObject = GetComponent<APooledObject<ABullet>>();
    }

    public abstract void OnFixedUpdate();
    protected abstract void OnBulletCollision();

    public abstract Type GetBulletType();
}
