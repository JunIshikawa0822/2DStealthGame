using System;
using UnityEngine;
using System.Threading;
using Unity.VisualScripting;
public class Bullet_10mm : ABullet
{
    [SerializeField]
    float _LifeDistance;
    private CancellationTokenSource bulletLifeCTS;

    void Awake()
    {
        OnSetUp(_LifeDistance);

        bulletLifeCTS = new CancellationTokenSource();
        BulletLifeTime();
    }
    void Start()
    {

    }
    //弾の当たり判定はFixedUpdate内で計算。
    void FixedUpdate()
    {
        if(IsBulletCollide())
        {
            Debug.Log("衝突");
            Debug.Log(GetBulletRaycastHit().collider.gameObject.name);
            bulletLifeCTS.Cancel();
            Release(this);
        }
    }

    protected override async void BulletLifeTime()
    {
        await Timer(1, bulletLifeCTS.Token);

        Release(this);
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_10mm);
    }
}
