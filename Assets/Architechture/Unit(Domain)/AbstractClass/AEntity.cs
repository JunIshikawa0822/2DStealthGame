using System.Collections.Generic;
using UnityEngine;

public abstract class AEntity : MonoBehaviour, IEntity
{
    protected int entityHP;
    protected List<IItem> _items;

    protected Rigidbody _entityRigidbody;
    protected Transform _entityTransform;

    public virtual void OnSetUp()
    {
        _entityRigidbody = GetComponent<Rigidbody>();
        _entityTransform = GetComponent<Transform>();
    }

    public virtual void OnUpdate()
    {

    }

    protected void EntityDead()
    {
        
    }
}
