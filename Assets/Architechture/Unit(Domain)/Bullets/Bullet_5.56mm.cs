using System;
using UnityEngine;
using System.Threading;

public class Bullet_5_56mm : ABullet, IItem
{
    [SerializeField]
    float _LifeDistance;
    private CancellationTokenSource bulletLifeCTS;
    private Action<Bullet_5_56mm> poolAction;

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
        return typeof(Bullet_5_56mm);
    }
}
