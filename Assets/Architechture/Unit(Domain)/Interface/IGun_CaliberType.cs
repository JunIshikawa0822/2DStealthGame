public interface IGun<T> : IItem where T : IPooledObject<T>
{
    public void OnSetUp(IFactory<Bullet_10mm> bulletFactory, IObjectPool<T> objectPool);
}
