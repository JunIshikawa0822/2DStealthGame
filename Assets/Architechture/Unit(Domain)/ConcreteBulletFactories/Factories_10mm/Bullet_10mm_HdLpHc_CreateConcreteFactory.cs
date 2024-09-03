using UnityEngine;
public class Bullet_10mm_HdLpHc_CreateConcreteFactory : IBulletFactory
{
    Bullet_10mm_HdLpHc bullet;

    public Bullet_10mm_HdLpHc_CreateConcreteFactory(Bullet_10mm_HdLpHc bullet)
    {
        this.bullet = bullet;
    }

    public ABullet BulletObjectInstantiate()
    {
        ABullet bulletInstance = GameObject.Instantiate(bullet);
        return bulletInstance;
    }
}
