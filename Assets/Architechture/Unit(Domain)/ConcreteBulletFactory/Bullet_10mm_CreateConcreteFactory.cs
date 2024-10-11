using UnityEngine;
using System;
public class Bullet_10mm_CreateConcreteFactory : IFactory<Bullet_10mm>
{
    Bullet_10mm bullet;

    public Bullet_10mm_CreateConcreteFactory(Bullet_10mm bullet)
    {
        this.bullet = bullet;
    }

    public Bullet_10mm ObjectInstantiate(IData data)
    {
        Bullet_10mm bulletInstance = GameObject.Instantiate(bullet);
        //bulletInstance.OnSetUp();
        
        return bulletInstance;
    }

    public Type GetFactoryType()
    {
        return typeof(Bullet_10mm);
    }
}
