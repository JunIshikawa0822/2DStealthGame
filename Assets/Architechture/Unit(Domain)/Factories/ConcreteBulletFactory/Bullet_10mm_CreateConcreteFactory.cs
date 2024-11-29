using UnityEngine;
using System;
public class Bullet_10mm_CreateConcreteFactory : IFactory
{
    Bullet_10mm _bullet;

    public Bullet_10mm_CreateConcreteFactory(Bullet_10mm bullet)
    {
        _bullet = bullet;
    }

    public IItem ObjectInstantiate()
    {
        Bullet_10mm bulletInstance = GameObject.Instantiate(_bullet);
        //bulletInstance.OnSetUp();
        
        return bulletInstance as IItem;
    }

    public IItem ObjectInstantiate(A_Item_Data data)
    {
        Bullet_10mm bulletInstance = GameObject.Instantiate(_bullet);
        //bulletInstance.OnSetUp();
        
        return bulletInstance as IItem;
    }

    public Type GetFactoryType()
    {
        return typeof(Bullet_10mm);
    }
}
