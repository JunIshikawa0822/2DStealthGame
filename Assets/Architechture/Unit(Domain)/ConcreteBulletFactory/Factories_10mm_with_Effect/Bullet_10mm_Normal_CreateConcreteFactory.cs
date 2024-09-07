using UnityEngine;
public class Bullet_10mm_Normal_CreateConcreteFactory : IBulletFactory
{
    Bullet_10mm_Normal bullet;

    public Bullet_10mm_Normal_CreateConcreteFactory(Bullet_10mm_Normal bullet)
    {
        this.bullet = bullet;
    }

    public ABullet BulletObjectInstantiate()
    {
        ABullet bulletInstance = GameObject.Instantiate(bullet);
        return bulletInstance;
    }
}
