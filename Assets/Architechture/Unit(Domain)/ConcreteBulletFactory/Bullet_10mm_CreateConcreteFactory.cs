using UnityEngine;
using System;
public class Bullet_10mm_CreateConcreteFactory : IFactory
{
    Bullet_10mm bullet;

    public Bullet_10mm_CreateConcreteFactory(Bullet_10mm bullet)
    {
        this.bullet = bullet;
    }

    public IItem ObjectInstantiate()
    {
        Bullet_10mm bulletInstance = GameObject.Instantiate(bullet);
        //bulletInstance.OnSetUp();
        
        return bulletInstance as IItem;
    }

    public Type GetFactoryType()
    {
        return typeof(Bullet_10mm);
    }
}
