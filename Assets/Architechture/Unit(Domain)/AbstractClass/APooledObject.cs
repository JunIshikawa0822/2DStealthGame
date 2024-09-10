using System;
using UnityEngine;

public abstract class APooledObject : MonoBehaviour
{
    public event Action<APooledObject> pooledObjectAction;

    protected Rigidbody _objectRigidbody;
    protected Transform _objectTransform;
    
    public Rigidbody GetObjectRigidbody(){return _objectRigidbody;}
    public Transform GetObjectTransform(){return _objectTransform;}
    protected void Release()
    {
        if (pooledObjectAction == null) return;
        pooledObjectAction(this);
    }
}
