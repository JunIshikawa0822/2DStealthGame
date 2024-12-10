using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class O_ObjectPool<T> : IObjectPool<T> where T : MonoBehaviour, IPooledObject<T>
{
    private Stack<IPooledObject<T>> _pool;
    private GameObject _parent;
    private Transform _transform;
    private IFactory _factory;

    public O_ObjectPool(Transform poolTransform, IFactory factory)
    {
        _pool = new Stack<IPooledObject<T>>();
        _parent = new GameObject($"{typeof(T).ToString()}");
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

        Stack<IPooledObject<T>> objectPool = _pool;
        GameObject poolParent = _parent;

        //とりあえずPoolSize分instanceを生成して、見えなくしておく
        for (int i = 0; i < initPoolSize; i++)
        {
            T instance = ObjectInstantiate();
            if(instance == null)return;

            instance.gameObject.transform.SetParent(poolParent.transform);
            instance.gameObject.SetActive(false);
            objectPool.Push(instance);
        }
    }

    public T GetFromPool()
    {
        if(_factory == null)
        {
            return null;
        }

        Stack<IPooledObject<T>> objectPool = _pool;
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
        IPooledObject<T> instance = _factory.ObjectInstantiate() as IPooledObject<T>;
        if(instance == null)return null;

        instance.SetPoolAction(ReturnToPool);
        return instance as T;
    }
    
    // 弾をプールに戻す
    public void ReturnToPool(T pooledObject)
    {
        Stack<IPooledObject<T>> objectPool = _pool;

        objectPool.Push(pooledObject);
        pooledObject.gameObject.SetActive(false);
    }
}
