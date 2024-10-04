using System;
using UnityEngine;
using System.Threading;
using Unity.VisualScripting;
public class Bullet_10mm : ABullet
{
    [SerializeField]
    float _lifeDistance;

    [SerializeField]
    float _bulletDamage;

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
            //Debug.Log("距離によって破壊");
            //Debug.Log($"距離で削除された時のPrePos : {_bulletPrePos}");
            Release(this);
        }
        else if(IsBulletCollide())
        {
            //Debug.Log("衝突によって破壊");
            //Debug.Log(GetBulletRaycastHit().collider.gameObject.name);

            OnBulletCollide(GetBulletRaycastHit().collider, _bulletDamage);
            
            Release(this);
        }
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_10mm);
    }
}
