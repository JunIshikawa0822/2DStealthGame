using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPool
{
    void PoolSetUp(IBulletFactory factory);
    APooledObject GetFromPool(IBulletFactory factory);
    void ReturnToPool(APooledObject pooledObject);
}
