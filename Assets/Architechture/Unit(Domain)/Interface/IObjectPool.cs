public interface IObjectPool<T> where T : IPooledObject<T>
{
    void PoolSetUp(IFactory<T> factory, uint index);
    T GetFromPool(IFactory<T> factory);
    void ReturnToPool(T pooledObject);
}
