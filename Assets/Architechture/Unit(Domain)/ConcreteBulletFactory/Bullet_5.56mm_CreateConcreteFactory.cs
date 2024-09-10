using UnityEngine;
public class Bullet_5_56mm_CreateConcreteFactory : IFactory<Bullet_5_56mm>
{
    Bullet_5_56mm bullet;

    public Bullet_5_56mm_CreateConcreteFactory(Bullet_5_56mm bullet)
    {
        this.bullet = bullet;
    }

    public Bullet_5_56mm ObjectInstantiate()
    {
        Bullet_5_56mm bulletInstance = GameObject.Instantiate(bullet);
        bullet.OnSetUp();
        
        return bulletInstance;
    }
}
