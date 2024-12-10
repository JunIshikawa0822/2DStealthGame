using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ObjectPool<T> : IObjectPool where T : APooledObject
{
    private Stack<APooledObject> _pool;
    private GameObject _parent;
    private Transform _transform;
    private IFactory _factory;

    public ObjectPool(Transform poolTransform, IFactory factory)
    {
        _pool = new Stack<APooledObject>();
        _parent = new GameObject(typeof(T).Name);
        _transform = poolTransform;

        _factory = factory;

        _parent.transform.SetParent(_transform);
    }

    public void PoolSetUp(uint initPoolSize)
    {
        //Stackの初期化
        if (_factory == null)
        {
            return;
        }

        Stack<APooledObject> objectPool = _pool;
        GameObject poolParent = _parent;

        //とりあえずPoolSize分instanceを生成して、見えなくしておく
        for (int i = 0; i < initPoolSize; i++)
        {
            T instance = ObjectInstantiate();
            // Debug.Log(instance);
            if(instance == null)return;

            instance.gameObject.transform.SetParent(poolParent.transform);
            instance.gameObject.SetActive(false);
            objectPool.Push(instance);
        }
    }

    public APooledObject GetFromPool()
    {
        if(_factory == null)
        {
            return null;
        }

        Stack<APooledObject> objectPool = _pool;
        GameObject poolParent = _parent;

        // プールに在庫があればそれを使用、なければ新規作成
        if (objectPool.Count < 1)
        {
            T newInstance = ObjectInstantiate();
            if(newInstance == null)return null;

            newInstance.gameObject.transform.SetParent(poolParent.transform);
            return newInstance;
        }

        T nextInstance = objectPool.Pop() as T;
        nextInstance.gameObject.SetActive(true);
        return nextInstance;
    }

    public T ObjectInstantiate()
    {
        T instance = _factory.ObjectInstantiate() as T;
        if(instance == null)return null;

        instance.SetPoolAction(ReturnToPool);
        return instance;
    }
    
    // 弾をプールに戻す
    public void ReturnToPool(APooledObject pooledObject)
    {
        Stack<APooledObject> objectPool = _pool;

        objectPool.Push(pooledObject);
        pooledObject.gameObject.SetActive(false);
    }
}
