using System;
using UnityEngine;
using System.Threading;

public class Bullet_7_62mm : ABullet
{
    [SerializeField]
    float _LifeDistance;
    private CancellationTokenSource bulletLifeCTS;

    public void Awake()
    {
        OnSetUp(_LifeDistance);

        bulletLifeCTS = new CancellationTokenSource();
        //BulletLifeTime();
    }
    public void Start()
    {

    }
    public void FixedUpdate()
    {
        if(IsBulletCollide())
        {
            Debug.Log("衝突");
        }
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_7_62mm);
    }
}
