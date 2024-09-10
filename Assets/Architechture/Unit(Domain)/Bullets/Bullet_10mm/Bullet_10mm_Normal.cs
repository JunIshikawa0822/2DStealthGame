
using System;

public class Bullet_10mm_Normal : ABullet
{
    public override void OnFixedUpdate()
    {

    }

    protected override void OnBulletCollision()
    {
        
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_10mm_Normal);
    }
}
