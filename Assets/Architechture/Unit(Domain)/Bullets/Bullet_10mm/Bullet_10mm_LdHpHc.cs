using System;
using System.Threading;
public class Bullet_10mm_LdHpHc : ABullet
{
    private CancellationTokenSource bulletLifeCTS;
    // protected override async void BulletLifeTime()
    // {
    //     await Timer(1, bulletLifeCTS.Token);

    //     Release(this);
    // }
    public override Type GetBulletType()
    {
        return typeof(Bullet_10mm_LdHpHc);
    }
}
