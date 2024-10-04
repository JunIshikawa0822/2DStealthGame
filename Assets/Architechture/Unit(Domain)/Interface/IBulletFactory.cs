public interface IBulletFactory<out TBullet> where TBullet : ABullet
{
    TBullet BulletInstantiate();
}

