public interface IGun<T> : IGun where T : IPooledObject<T> 
{
    public void OnSetUp(IFactory<T> bulletFactory, IObjectPool<T> objectPool);
}
