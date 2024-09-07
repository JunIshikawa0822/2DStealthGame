using UnityEngine;
public class Bullet_10mm_LdLpLc_CreateConcreteFactory : IBulletFactory
{
    Bullet_10mm_LdLpLc bullet;

    public Bullet_10mm_LdLpLc_CreateConcreteFactory(Bullet_10mm_LdLpLc bullet)
    {
        this.bullet = bullet;
    }

    public ABullet BulletObjectInstantiate()
    {
        ABullet bulletInstance = GameObject.Instantiate(bullet);
        return bulletInstance;
    }
}
