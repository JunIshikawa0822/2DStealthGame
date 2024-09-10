using System;

public class Bullet_7_72mm : ABullet
{
    public override void OnFixedUpdate()
    {
        
    }

    protected override void OnBulletCollision()
    {
        
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_7_72mm);
    }
}
