using System;
using UnityEngine;
using System.Threading;

public class Bullet_7_62mm : ABullet, IObject
{
    [SerializeField]
    float _LifeDistance;
    private CancellationTokenSource bulletLifeCTS;

    private Action<Bullet_7_62mm> poolAction;
    public string Name{get; set;}

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
