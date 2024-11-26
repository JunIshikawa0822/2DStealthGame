using UnityEngine;
using System;
public class Bullet_5_56mm_CreateConcreteFactory : IFactory
{
    Bullet_5_56mm bullet;

    public Bullet_5_56mm_CreateConcreteFactory(Bullet_5_56mm bullet)
    {
        this.bullet = bullet;
    }

    public IItem ObjectInstantiate()
    {
        Bullet_5_56mm bulletInstance = GameObject.Instantiate(bullet);
        //bullet.OnSetUp();
        
        return bulletInstance as IItem;
    }

    public Type GetFactoryType()
    {
        return typeof(Bullet_5_56mm);
    }
}
