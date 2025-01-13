using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public abstract class AEntity : MonoBehaviour
{
    protected Entity_HealthPoint _entityHP;
    //protected List<IItem> _items;

    protected Rigidbody _entityRigidbody;
    protected Transform _entityTransform;

    protected MeshRenderer _entityRenderer;
    protected MeshRenderer[] _entityChildrenMeshsArray;

    protected bool _isEntityActionInterval;
    protected CancellationTokenSource _actionCancellationTokenSource;

    public abstract IStorage Storage{get;}

    public virtual void OnSetUp(Entity_HealthPoint entity_HealthPoint)
    {
        _entityHP = entity_HealthPoint;

        _entityRigidbody = GetComponent<Rigidbody>();
        _entityTransform = GetComponent<Transform>();

        _entityRenderer = GetComponent<MeshRenderer>();
        _entityChildrenMeshsArray= GetComponentsInChildren<MeshRenderer>();  
        _actionCancellationTokenSource = new CancellationTokenSource();
    }

    public virtual void OnUpdate()
    {

    }

    // public bool IsEntityDead()
    // {
    //     if(_entityHP.CurrentHp <= 0) return true;
    //     else return false;
    // }
    public virtual bool IsEntityDead()
    {
        if(_entityHP.CurrentHp <= 0)return true;
        return false;
    }
    public virtual void OnEntityDead()
    {
        Debug.Log($"プレイヤー({this.gameObject.name})はやられた！");
    }

    public abstract void OnDamage(float damage);

    public Transform GetEntityTransform(){return this._entityTransform;}

    public Rigidbody GetEntityRigidbody(){return this._entityRigidbody;}

    public void EntityMeshDisable()
    {
        Debug.Log(_entityRenderer);
        
        _entityRenderer.enabled = false;

        foreach(MeshRenderer mesh in _entityChildrenMeshsArray)
        {
            mesh.enabled = false;
        }
    }

    public void EntityMeshAble()
    {
        Debug.Log(_entityRenderer);

        _entityRenderer.enabled = true;

        foreach(MeshRenderer mesh in _entityChildrenMeshsArray)
        {
            mesh.enabled = true;
        }
    }

    public async UniTask EntityActionInterval(Action waitAction, CancellationToken token, float time, string ActionName)
    {
        _isEntityActionInterval = true;

        try
        {
            // 指定されたクールタイム期間を待つ (キャンセル可能)
            Debug.Log($"{ActionName} 開始");
            await UniTask.Delay((int)(time * 1000), cancellationToken: token);
            waitAction?.Invoke();
            Debug.Log($"{ActionName} 終了");
        }
        catch
        {
            Debug.Log($"{ActionName} がキャンセルされました");
        }
        finally
        {
            _isEntityActionInterval = false; // クールタイム終了（またはキャンセル)
        }
    }
}
