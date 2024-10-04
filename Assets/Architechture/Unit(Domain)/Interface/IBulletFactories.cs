using System;

public interface IBulletFactories
{
    //public void SetUp();
    public IFactory<ABullet> BulletFactory(Type type);
}
