using System.Collections.Generic;
using UnityEngine;

public abstract class AEntity : MonoBehaviour
{
    protected Entity_HealthPoint _entityHP;
    protected List<IItem> _items;

    protected Rigidbody _entityRigidbody;
    protected Transform _entityTransform;

    protected MeshRenderer _entityRenderer;
    protected MeshRenderer[] _entityChildrenMeshsArray;

    public void EntitySetUp(Entity_HealthPoint entityHP)
    {
        _entityRigidbody = GetComponent<Rigidbody>();
        _entityTransform = GetComponent<Transform>();

        _entityRenderer = GetComponent<MeshRenderer>();
        _entityChildrenMeshsArray= GetComponentsInChildren<MeshRenderer>();

        _entityHP = entityHP;
    }

    public virtual void OnUpdate()
    {

    }

    public bool IsEntityDead()
    {
        if(_entityHP.CurrentHp <= 0) return true;
        else return false;
    }

    public abstract void OnEntityDead();
    public abstract void OnDamage(float damage);

    public Transform GetEntityTransform(){return this._entityTransform;}

    public Rigidbody GetEntityRigidbody(){return this._entityRigidbody;}

    public abstract void OnEntityMeshDisable();

    public abstract void OnEntityMeshAble();
}
