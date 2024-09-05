using UnityEngine;
public class Bullet_5_56mm_CreateConcreteFactory : IBulletFactory
{
    Bullet_5_56mm bullet;

    public Bullet_5_56mm_CreateConcreteFactory(Bullet_5_56mm bullet)
    {
        this.bullet = bullet;
    }

    public ABullet BulletObjectInstantiate()
    {
        ABullet bulletInstance = GameObject.Instantiate(bullet);
        return bulletInstance;
    }
}
