using System;
using UnityEngine;

public abstract class APooledObject<T> : MonoBehaviour
{
    public event Action<T> pooledObjectAction;

    protected void Release(T pooledObject)
    {
        if (pooledObjectAction == null) return;
        pooledObjectAction(pooledObject);
    }
}
