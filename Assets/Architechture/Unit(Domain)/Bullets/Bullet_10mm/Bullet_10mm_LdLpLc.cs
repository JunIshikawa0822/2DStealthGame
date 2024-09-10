using System;
using Unity.VisualScripting;

public class Bullet_10mm_LdLpLc : ABullet
{
    public override void OnFixedUpdate()
    {
        
    }

    protected override void OnBulletCollision()
    {
        
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_10mm_LdLpLc);
    }
}
