using UnityEngine;
using System;
public class Bullet_5_56mm_CreateConcreteFactory : IFactory
{
    Bullet_5_56mm _bullet;

    public Bullet_5_56mm_CreateConcreteFactory(Bullet_5_56mm bullet)
    {
        _bullet = bullet;
    }

    public IObject ObjectInstantiate()
    {
        Bullet_5_56mm bulletInstance = GameObject.Instantiate(_bullet);
        //bullet.OnSetUp();
        
        return bulletInstance as IObject;
    }

    public IObject ObjectInstantiate(A_Item_Data data)
    {
        Bullet_5_56mm bulletInstance = GameObject.Instantiate(_bullet);
        //bullet.OnSetUp();
        
        return bulletInstance as IObject;
    }

    public IObject ObjectInstantiate(I_Data_Item data)
    {
        Bullet_5_56mm bulletInstance = GameObject.Instantiate(_bullet);
        //bullet.OnSetUp();
        
        return bulletInstance as IObject;
    }

}
