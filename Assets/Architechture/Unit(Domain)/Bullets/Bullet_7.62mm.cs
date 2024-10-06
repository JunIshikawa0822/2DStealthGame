using System;
using UnityEngine;
using System.Threading;

public class Bullet_7_62mm : ABullet, IPooledObject<Bullet_7_62mm>
{
    [SerializeField]
    float _lifeDistance;

    [SerializeField]
    float _bulletDamage;

    private Action<Bullet_7_62mm> poolEvent;

    void Awake()
    {
        OnSetUp(_lifeDistance);
    }

    //弾の当たり判定はFixedUpdate内で計算。
    void FixedUpdate()
    {
        //Debug.Log($"Distance{_bulletLifeDistance}");
        if(IsBeyondLifeDistance())
        {
            Debug.Log("距離によって破壊");
            //Debug.Log($"距離で削除された時のPrePos : {_bulletPrePos}");
            Release(this);
        }
        else if(IsBulletCollide())
        {
            //Debug.Log("衝突によって破壊");

            Debug.Log($"{GetBulletRaycastHit().collider.name}にぶつかった");

            AEntity entity = GetBulletRaycastHit().collider.GetComponent<AEntity>();

            Release(this);

            if(entity == null)return;
            entity.OnDamage(_bulletDamage);
        }
    }

    #region ABulletとしての実装
    public override Type GetBulletType()
    {
        return typeof(Bullet_7_62mm);
    }
    #endregion

    #region PooledObjectとしての実装
    public void Release()
    {
        if(poolEvent == null)return;
        poolEvent?.Invoke(this);
    }

    public void SetPoolEvent(Action<Bullet_7_62mm> action)
    {
        poolEvent += action;
    }

    #endregion
}
