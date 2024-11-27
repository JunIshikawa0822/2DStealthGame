using UnityEngine;

public interface IObjectPool
{
    void PoolSetUp(uint index);
    APooledObject GetFromPool();
    void ReturnToPool(APooledObject pooledObject);
}
