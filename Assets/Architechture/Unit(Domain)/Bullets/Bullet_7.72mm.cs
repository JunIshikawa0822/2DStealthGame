using System;
using UnityEngine;

public class Bullet_7_72mm : ABullet
{
    [SerializeField]
    float _LifeDistance;
    public void Awake()
    {
        OnSetUp(30);
    }
    public void Start()
    {

    }
    public void FixedUpdate()
    {
        if(IsBulletCollide())
        {
            
        }
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_7_72mm);
    }
}
