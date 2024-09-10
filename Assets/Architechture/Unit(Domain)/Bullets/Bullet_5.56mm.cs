using System;

public class Bullet_5_56mm : ABullet
{
    public override void OnFixedUpdate()
    {
        
    }

    protected override void OnBulletCollision()
    {
        
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_5_56mm);
    }
}
