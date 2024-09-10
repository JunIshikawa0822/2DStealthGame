using UnityEngine;

public abstract class  ABullet : APooledObject, IOnFixedUpdate
{
    protected int _bulletDamage;

    public void OnSetUp()
    {
        //this._bulletDamage = bulletDamage;
        _objectRigidbody = GetComponent<Rigidbody>();
        _objectTransform = GetComponent<Transform>();
    }

    public abstract void OnFixedUpdate();
    protected abstract void OnBulletCollision();
}
