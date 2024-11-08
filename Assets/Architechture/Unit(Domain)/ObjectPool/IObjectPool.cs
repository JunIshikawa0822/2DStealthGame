public interface IObjectPool<T> where T : IPooledObject<T>
{
    void PoolSetUp(uint index);
    T GetFromPool();
    void ReturnToPool(T pooledObject);
}
