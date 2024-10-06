using System;

public interface IFactories<T>
{
    //public void SetUp();
    public IFactory<T> BulletFactory(Type type);
}
