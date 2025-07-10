using UnityEngine;
using System;
public class Bullet_Rocket_CreateConcreteFactory : IFactory
{
    Bullet_Rocket _bullet;

    public Bullet_Rocket_CreateConcreteFactory(Bullet_Rocket bullet)
    {
        _bullet = bullet;
    }

    public IObject ObjectInstantiate()
    {
        Bullet_Rocket bulletInstance = GameObject.Instantiate(_bullet);
        //bulletInstance.OnSetUp();

        return bulletInstance as IObject;
    }

    public IObject ObjectInstantiate(A_Item_Data data)
    {
        Bullet_Rocket bulletInstance = GameObject.Instantiate(_bullet);
        //bulletInstance.OnSetUp();

        return bulletInstance as IObject;
    }

    public IObject ObjectInstantiate(I_Data_Item data)
    {
        Bullet_Rocket bulletInstance = GameObject.Instantiate(_bullet);
        //bulletInstance.OnSetUp();

        return bulletInstance as IObject;
    }
}
