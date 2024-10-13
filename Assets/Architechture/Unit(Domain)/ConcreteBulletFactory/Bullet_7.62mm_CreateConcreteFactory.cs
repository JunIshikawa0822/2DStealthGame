using UnityEngine;
using System;
public class Bullet_7_62mm_CreateConcreteFactory : IFactory<Bullet_7_62mm>
{
    Bullet_7_62mm bullet;

    public Bullet_7_62mm_CreateConcreteFactory(Bullet_7_62mm bullet)
    {
        this.bullet = bullet;
    }

    public Bullet_7_62mm ObjectInstantiate(IData data)
    {
        Bullet_7_62mm bulletInstance = GameObject.Instantiate(bullet);
        //bulletInstance.OnSetUp();
        
        return bulletInstance;
    }

    public Type GetFactoryType()
    {
        return typeof(Bullet_7_62mm);
    }
}
