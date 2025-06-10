using System;
using UnityEngine;
using System.Threading;

public class Bullet_7_62mm : ABullet, IObject
{
    [SerializeField]
    float _lifeDistance;
    [SerializeField]
    float _bulletDamage;
    private Action<Bullet_7_62mm> poolAction;
    public string Name{get; set;}

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
            // Debug.Log("距離によって破壊");
            //Debug.Log($"距離で削除された時のPrePos : {_bulletPrePos}");
            Release();
        }
        else if(IsBulletCollide())
        {
            Debug.Log($"{GetBulletRaycastHit().collider.name}にぶつかって破壊");

            AEntity entity = GetBulletRaycastHit().collider.GetComponent<AEntity>();

            Release();

            if(entity == null)return;
            entity.OnDamage(_bulletDamage);
        }
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_10mm);
    }
}
