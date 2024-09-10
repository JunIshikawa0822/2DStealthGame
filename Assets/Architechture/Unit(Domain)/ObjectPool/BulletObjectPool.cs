using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Pool;
public class BulletObjectPool : MonoBehaviour, IObjectPool<ABullet>
{
    private uint initPoolSize;
    private Dictionary<Type, Stack<APooledObject<ABullet>>> poolsDic = new Dictionary<Type, Stack<APooledObject<ABullet>>>();
    private Dictionary<Type, GameObject> parentDic = new Dictionary<Type, GameObject>();

    public void PoolSetUp(IFactory<ABullet> factory, uint initPoolSize)
    {
        this.initPoolSize = initPoolSize;
        //Stackの初期化

        if (factory == null)
        {
            return;
        }

        Type factoryType = factory.GetFactoryType();

        // もし該当の弾のプールが存在しない場合、新しく作成
        if (!poolsDic.ContainsKey(factoryType))
        {
            poolsDic[factoryType] = new Stack<APooledObject<ABullet>>();
            parentDic[factoryType] = new GameObject(factoryType.ToString() + "_pool_parent");
            parentDic[factoryType].transform.SetParent(this.transform);
        }

        Stack<APooledObject<ABullet>> bulletPool = poolsDic[factoryType];
        GameObject poolParent = parentDic[factoryType];

        //とりあえずPoolSize分instanceを生成して、見えなくしておく
        for (int i = 0; i < initPoolSize; i++)
        {
            APooledObject<ABullet> instance = ObjectInstantiate(factory);

            instance.gameObject.transform.parent = poolParent.transform;
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
        if (!poolsDic.ContainsKey(factoryType))
        {
            poolsDic[factoryType] = new Stack<APooledObject<ABullet>>();
            parentDic[factoryType] = new GameObject(factoryType.ToString() + "_pool_parent");
            parentDic[factoryType].transform.SetParent(this.transform);
        }

        Stack<APooledObject<ABullet>> bulletPool = poolsDic[factoryType];
        GameObject poolParent = parentDic[factoryType];

        // プールに弾があればそれを使用、なければ新規作成
        if (bulletPool.Count < 1)
        {
            APooledObject<ABullet> newInstance = ObjectInstantiate(factory);
            newInstance.gameObject.transform.parent = poolParent.transform;
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

        if (!poolsDic.ContainsKey(bulletType))
        {
            poolsDic[bulletType] = new Stack<APooledObject<ABullet>>();
        }

        Stack<APooledObject<ABullet>> bulletPool = poolsDic[bulletType];

        bulletPool.Push(bullet);
        bullet.gameObject.SetActive(false);
    }
}
