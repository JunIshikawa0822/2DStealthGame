using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Pool;
public class BulletObjectPool : IObjectPool<ABullet>
{
    //private uint _initPoolSize;
    private Dictionary<Type, Stack<APooledObject<ABullet>>> _poolsDic;
    private Dictionary<Type, GameObject> _parentDic;
    private Transform _poolTransform;

    public BulletObjectPool(Transform poolTransform)
    {
        this._poolTransform = poolTransform;
        this._poolsDic = new Dictionary<Type, Stack<APooledObject<ABullet>>>();
        this._parentDic = new Dictionary<Type, GameObject>();
    }

    public void PoolSetUp(IFactory<ABullet> factory, uint initPoolSize)
    {
        //Stackの初期化
        if (factory == null)
        {
            return;
        }

        Type factoryType = factory.GetFactoryType();

        // もし該当の弾のプールが存在しない場合、新しく作成
        if (!_poolsDic.ContainsKey(factoryType))
        {
            _poolsDic[factoryType] = new Stack<APooledObject<ABullet>>();
            _parentDic[factoryType] = new GameObject(factoryType.ToString() + "_pool_parent");
            _parentDic[factoryType].transform.SetParent(_poolTransform);
        }

        Stack<APooledObject<ABullet>> bulletPool = _poolsDic[factoryType];
        GameObject poolParent = _parentDic[factoryType];

        //とりあえずPoolSize分instanceを生成して、見えなくしておく
        for (int i = 0; i < initPoolSize; i++)
        {
            APooledObject<ABullet> instance = ObjectInstantiate(factory);

            instance.gameObject.transform.SetParent(poolParent.transform);
            instance.gameObject.SetActive(false);
            bulletPool.Push(instance);
        }
    }

    public APooledObject<ABullet> GetFromPool(IFactory<ABullet> factory)
    {
        if(factory == null)
        {
            return null;
        }

        Type factoryType = factory.GetFactoryType();

        // もし該当の弾のプールが存在しない場合、新しく作成
        if (!_poolsDic.ContainsKey(factoryType))
        {
            _poolsDic[factoryType] = new Stack<APooledObject<ABullet>>();
            _parentDic[factoryType] = new GameObject(factoryType.ToString() + "_pool_parent");
            _parentDic[factoryType].transform.SetParent(_poolTransform);
        }

        Stack<APooledObject<ABullet>> bulletPool = _poolsDic[factoryType];
        GameObject poolParent = _parentDic[factoryType];

        // プールに弾があればそれを使用、なければ新規作成
        if (bulletPool.Count < 1)
        {
            APooledObject<ABullet> newInstance = ObjectInstantiate(factory);
            newInstance.gameObject.transform.SetParent(poolParent.transform);
            return newInstance;
        }

        APooledObject<ABullet> nextInstance = bulletPool.Pop();
        nextInstance.gameObject.SetActive(true);
        return nextInstance;
    }

    public APooledObject<ABullet> ObjectInstantiate(IFactory<ABullet> factory)
    {
        APooledObject<ABullet> instance = factory.ObjectInstantiate();
        instance.pooledObjectAction += ReturnToPool;
        return instance;
    }
    
    // 弾をプールに戻す
    public void ReturnToPool(ABullet bullet)
    {
        Type bulletType = bullet.GetBulletType();

        if (!_poolsDic.ContainsKey(bulletType))
        {
            _poolsDic[bulletType] = new Stack<APooledObject<ABullet>>();
        }

        Stack<APooledObject<ABullet>> bulletPool = _poolsDic[bulletType];

        bulletPool.Push(bullet);
        bullet.gameObject.SetActive(false);
    }
}
