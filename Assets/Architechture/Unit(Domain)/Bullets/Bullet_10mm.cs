using System;
using UnityEngine;
public class Bullet_10mm : ABullet
{
    [SerializeField]
    float _LifeDistance;
    public void Awake()
    {
        OnSetUp(_LifeDistance);
    }
    public void Start()
    {

    }
    //弾の当たり判定はFixedUpdate内で計算。
    public void FixedUpdate()
    {
        if(IsBeyondLifeDistance())
        {
            Debug.Log("一定距離飛んだよ");
            Release(this);
        }

        if(IsBulletCollide())
        {
            Debug.Log("衝突");
            Debug.Log(GetBulletRaycastHit().collider.gameObject.name);
            Release(this);
        }
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_10mm);
    }
}
