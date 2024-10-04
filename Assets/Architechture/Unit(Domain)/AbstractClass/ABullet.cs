using UnityEngine;
using System;

public abstract class ABullet : APooledObject<ABullet>
{
    //public Action<Collider, float> onBulletCollisionEvent;
    private Rigidbody _bulletRigidbody;
    private Transform _bulletTransform;
    private Vector3 _bulletPrePos;
    private RaycastHit _bulletHit;
    protected float _bulletLifeDistance;
    private float _bulletLifeMaxDistance;

    //----------------------------------------------------------

    //private IEffect _bulletEffect;

    //----------------------------------------------------------

    public Rigidbody GetBulletRigidbody(){return _bulletRigidbody;}
    public Transform GetBulletTransform(){return _bulletTransform;}
    public RaycastHit GetBulletRaycastHit(){return _bulletHit;}

    public void OnSetUp(float bulletLifeMaxDistance)
    {
        _bulletRigidbody = GetComponent<Rigidbody>();
        _bulletTransform = GetComponent<Transform>();

        _bulletLifeMaxDistance = bulletLifeMaxDistance;
    }

    public void Init(Vector3 position)
    {
        //Debug.Log("Init");
        _bulletPrePos = position;
        _bulletRigidbody.position = position;

        _bulletRigidbody.velocity = Vector3.zero;
        _bulletRigidbody.angularVelocity = Vector3.zero;
        _bulletLifeDistance = 0;
    }

    protected bool IsBeyondLifeDistance()
    {
        Vector3 bulletNowPos = _bulletRigidbody.position;
        float bulletMoveDistance = (bulletNowPos - _bulletPrePos).magnitude;
        _bulletLifeDistance += bulletMoveDistance;

        bool isBeyondLifeDistance = false;

        if(_bulletLifeDistance > _bulletLifeMaxDistance)
        {
            _bulletLifeDistance = 0;
            isBeyondLifeDistance = true;
        }

        return isBeyondLifeDistance;
    }
    protected bool IsBulletCollide()
    {
        //今フレームでの位置
        Vector3 bulletNowPos = _bulletRigidbody.position;
        Vector3 bulletMoveVec = bulletNowPos - _bulletPrePos;
        bool isBulletCollide = false;

        if (Physics.Raycast(bulletNowPos, bulletMoveVec, out RaycastHit hit, bulletMoveVec.magnitude))
        {
            isBulletCollide = true;
            _bulletHit = hit;
        }

        //今のフレームの位置を次のフレームにおける前のフレームの位置として保存
        _bulletPrePos = bulletNowPos;
        return isBulletCollide;
    }

    protected void OnBulletCollide(Collider collider, float damage)
    {
        AEntity entity = collider.GetComponent<AEntity>();

        if(entity == null)return;
        entity.OnDamage(damage);
    }

    public abstract Type GetBulletType();
}
