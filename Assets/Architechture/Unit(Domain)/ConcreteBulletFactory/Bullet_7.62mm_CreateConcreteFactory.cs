using UnityEngine;
using System;
public class Bullet_7_62mm_CreateConcreteFactory : IFactory
{
    Bullet_7_62mm bullet;

    public Bullet_7_62mm_CreateConcreteFactory(Bullet_7_62mm bullet)
    {
        this.bullet = bullet;
    }

    public IItem ObjectInstantiate()
    {
        Bullet_7_62mm bulletInstance = GameObject.Instantiate(bullet);
        //bulletInstance.OnSetUp();
        
        return bulletInstance as IItem;
    }

    public Type GetFactoryType()
    {
        return typeof(Bullet_7_62mm);
    }
}
