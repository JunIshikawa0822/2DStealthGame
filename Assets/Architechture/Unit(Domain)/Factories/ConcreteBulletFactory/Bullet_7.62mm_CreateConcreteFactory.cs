using UnityEngine;
using System;
public class Bullet_7_62mm_CreateConcreteFactory : IFactory
{
    Bullet_7_62mm _bullet;

    public Bullet_7_62mm_CreateConcreteFactory(Bullet_7_62mm bullet)
    {
        _bullet = bullet;
    }

    public IItem ObjectInstantiate()
    {
        Bullet_7_62mm bulletInstance = GameObject.Instantiate(_bullet);
        //bulletInstance.OnSetUp();
        
        return bulletInstance as IItem;
    }

    public IItem ObjectInstantiate(A_Item_Data data)
    {
        Bullet_7_62mm bulletInstance = GameObject.Instantiate(_bullet);
        //bulletInstance.OnSetUp();
        
        return bulletInstance as IItem;
    }

    public Type GetFactoryType()
    {
        return typeof(Bullet_7_62mm);
    }
}
