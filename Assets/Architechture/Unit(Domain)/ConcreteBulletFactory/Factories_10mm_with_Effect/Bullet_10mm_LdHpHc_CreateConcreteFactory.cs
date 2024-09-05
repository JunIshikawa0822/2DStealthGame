using UnityEngine;
public class Bullet_10mm_LdHpHc_CreateConcreteFactory : IBulletFactory
{
    Bullet_10mm_LdHpHc bullet;

    public Bullet_10mm_LdHpHc_CreateConcreteFactory(Bullet_10mm_LdHpHc bullet)
    {
        this.bullet = bullet;
    }

    public ABullet BulletObjectInstantiate()
    {
        ABullet bulletInstance = GameObject.Instantiate(bullet);
        return bulletInstance;
    }
}
