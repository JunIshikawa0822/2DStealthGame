using System.Collections.Generic;
using UnityEngine;

public abstract class AEntity : MonoBehaviour
{
    protected int entityHP;
    protected List<IItem> _items;

    protected Rigidbody _entityRigidbody;
    protected Transform _entityTransform;

    public virtual void OnSetUp(int entityHP)
    {
        _entityRigidbody = GetComponent<Rigidbody>();
        _entityTransform = GetComponent<Transform>();

        this.entityHP = entityHP;
    }

    public virtual void OnUpdate()
    {

    }

    public bool IsEntityDead()
    {
        if(entityHP < 1) return true;
        else return false;
    }

    public abstract void OnEntityDead();
    public abstract void OnDamage(float damage);

    public Transform GetEntityTransform()
    {
        return this._entityTransform;
    }

    public Rigidbody GetEntityRigidbody()
    {
        return this._entityRigidbody;
    }
}
