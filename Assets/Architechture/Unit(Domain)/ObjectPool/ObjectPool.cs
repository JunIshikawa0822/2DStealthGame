using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ObjectPool<T> : IObjectPool<T> where T : MonoBehaviour, IPooledObject<T>
{
    private Stack<IPooledObject<T>> _pool;
    private GameObject _parent;
    private Transform _transform;

    public ObjectPool(Transform poolTransform)
    {
        _pool = new Stack<IPooledObject<T>>();
        _parent = new GameObject($"{typeof(T).ToString()}");
        _transform = poolTransform;

        _parent.transform.SetParent(_transform);
    }

    public void PoolSetUp(IFactory<T> factory, uint initPoolSize)
    {
        //Stackの初期化
        if (factory == null)
        {
            return;
        }

        Stack<IPooledObject<T>> objectPool = _pool;
        GameObject poolParent = _parent;

        //とりあえずPoolSize分instanceを生成して、見えなくしておく
        for (int i = 0; i < initPoolSize; i++)
        {
            T instance = ObjectInstantiate(factory);

            instance.gameObject.transform.SetParent(poolParent.transform);
            instance.gameObject.SetActive(false);
            objectPool.Push(instance);
        }
    }

    public T GetFromPool(IFactory<T> factory)
    {
        if(factory == null)
        {
            return null;
        }

        Stack<IPooledObject<T>> objectPool = _pool;
        GameObject poolParent = _parent;

        // プールに在庫があればそれを使用、なければ新規作成
        if (objectPool.Count < 1)
        {
            T newInstance = ObjectInstantiate(factory);
            newInstance.gameObject.transform.SetParent(poolParent.transform);
            return newInstance;
        }

        T nextInstance = objectPool.Pop() as T;
        nextInstance.gameObject.SetActive(true);
        return nextInstance;
    }

    public T ObjectInstantiate(IFactory<T> factory)
    {
        IPooledObject<T> instance = factory.ObjectInstantiate(null);
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
