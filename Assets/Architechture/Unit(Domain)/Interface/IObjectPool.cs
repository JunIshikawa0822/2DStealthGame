using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPool
{
    void SetUp();
    APooledObject GetObject(IBulletFactory factory);
    void ReturnToPool(APooledObject pooledObject);
}
