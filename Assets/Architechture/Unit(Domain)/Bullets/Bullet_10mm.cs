using System;
using UnityEngine;
using System.Threading;
using Unity.VisualScripting;
public class Bullet_10mm : ABullet
{
    [SerializeField]
    float _LifeDistance;

    void Awake()
    {
        OnSetUp(_LifeDistance);
    }
    void Start()
    {

    }

    //弾の当たり判定はFixedUpdate内で計算。
    void FixedUpdate()
    {
        //Debug.Log($"Distance{_bulletLifeDistance}");
        if(IsBeyondLifeDistance())
        {
            //Debug.Log($"距離で削除された時のPrePos : {_bulletPrePos}");
            Release(this);
        }
        else if(IsBulletCollide())
        {
            //Debug.Log($"衝突で破壊された時のPrePos : {_bulletPrePos}");
            Release(this);
        }
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_10mm);
    }
}
