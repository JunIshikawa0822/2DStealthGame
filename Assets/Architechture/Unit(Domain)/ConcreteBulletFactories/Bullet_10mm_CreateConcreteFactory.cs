using UnityEngine;
public class Bullet_10mm_CreateConcreteFactory : IBulletFactory
{
    Bullet_10mm bullet;

    public Bullet_10mm_CreateConcreteFactory(Bullet_10mm bullet)
    {
        this.bullet = bullet;
    }

    public ABullet BulletObjectInstantiate()
    {
        ABullet bulletInstance = GameObject.Instantiate(bullet);
        return bulletInstance;
    }
}
