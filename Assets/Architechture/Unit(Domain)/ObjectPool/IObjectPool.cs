using UnityEngine;

public interface IObjectPool<T> where T : MonoBehaviour, IPooledObject<T>
{
    void PoolSetUp(uint index);
    T GetFromPool();
    void ReturnToPool(T pooledObject);
}
