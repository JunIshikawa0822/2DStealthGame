using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ObjectPool<T> : IObjectPool<T> where T : MonoBehaviour, IPooledObject<T>
{
    private Stack<IPooledObject<T>> _pool;
    private GameObject _parent;
    private Transform _transform;

    private IFactory<T> _factory;

    public ObjectPool(Transform poolTransform, IFactory<T> factory)
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

        Stack<IPooledObject<T>> bulletPool = _pool;
        GameObject poolParent = _parent;

        //とりあえずPoolSize分instanceを生成して、見えなくしておく
        for (int i = 0; i < initPoolSize; i++)
        {
            T instance = ObjectInstantiate(_factory);

            instance.gameObject.transform.SetParent(poolParent.transform);
            instance.gameObject.SetActive(false);
            bulletPool.Push(instance);
        }
    }

    public T GetFromPool()
    {
        if(_factory == null)
        {
            return null;
        }

        Stack<IPooledObject<T>> bulletPool = _pool;
        GameObject poolParent = _parent;

        // プールに弾があればそれを使用、なければ新規作成
        if (bulletPool.Count < 1)
        {
            T newInstance = ObjectInstantiate(_factory);
            newInstance.gameObject.transform.SetParent(poolParent.transform);
            return newInstance;
        }

        T nextInstance = bulletPool.Pop() as T;
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
    public void ReturnToPool(T bullet)
    {
        Stack<IPooledObject<T>> bulletPool = _pool;

        bulletPool.Push(bullet);
        bullet.gameObject.SetActive(false);
    }
}
