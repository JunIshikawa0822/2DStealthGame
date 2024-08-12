using UnityEngine;
public class CreateConcrete_Bullet_10mm : IBulletFactory
{
    Bullet_10mm bullet;

    public CreateConcrete_Bullet_10mm(Bullet_10mm bullet)
    {
        this.bullet = bullet;
    }

    public ABullet BulletObjectInstantiate()
    {
        ABullet bulletInstance = GameObject.Instantiate(bullet);
        return bulletInstance;
    }
}
