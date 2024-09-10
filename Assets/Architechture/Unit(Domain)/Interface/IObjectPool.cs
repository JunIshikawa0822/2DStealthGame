public interface IObjectPool<T> where T : APooledObject<T>
{
    void PoolSetUp(IFactory<T> factory, uint index);
    APooledObject<T> GetFromPool(IFactory<T> factory);
    void ReturnToPool(T pooledObject);
}
