using System;
using Unity.VisualScripting;
using System.Threading;

public class Bullet_10mm_LdLpLc : ABullet
{
    private CancellationTokenSource bulletLifeCTS;
    protected override async void BulletLifeTime()
    {
        await Timer(1, bulletLifeCTS.Token);

        Release(this);
    }
    public override Type GetBulletType()
    {
        return typeof(Bullet_10mm_LdLpLc);
    }
}
