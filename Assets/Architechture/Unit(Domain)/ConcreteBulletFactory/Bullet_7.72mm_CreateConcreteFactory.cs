using UnityEngine;
using System;
public class Bullet_7_72mm_CreateConcreteFactory : IFactory<Bullet_7_72mm>
{
    Bullet_7_72mm bullet;

    public Bullet_7_72mm_CreateConcreteFactory(Bullet_7_72mm bullet)
    {
        this.bullet = bullet;
    }

    public Bullet_7_72mm ObjectInstantiate()
    {
        Bullet_7_72mm bulletInstance = GameObject.Instantiate(bullet);
        bulletInstance.OnSetUp();
        
        return bulletInstance;
    }

    public Type GetFactoryType()
    {
        return typeof(Bullet_10mm);
    }
}
