using System;
using UnityEngine;

public abstract class APooledObject : MonoBehaviour
{
    public event Action<APooledObject> pooledObjectAction;

    public void Release()
    {
        if (pooledObjectAction == null) return;
        pooledObjectAction(this);
    }
}
