public interface IObjectPool<T> where T : APooledObject
{
    void PoolSetUp(IFactory<T> factory, uint index);
    T GetFromPool(IFactory<T> factory);
    void ReturnToPool(T pooledObject);
}
