using System;
using UnityEngine;

public class Bullet_5_56mm : ABullet
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
