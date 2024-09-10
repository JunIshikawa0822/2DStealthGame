using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Pool;
public class BulletObjectPool : MonoBehaviour, IObjectPool<ABullet>
{
    private uint initPoolSize;

    //追加したものを、最後に追加した順に取り出せる、それ以外はListと同じ
    Dictionary<Type, Stack<ABullet>> poolsDictionary = new Dictionary<Type,  Stack<ABullet>>();

    private Stack<ABullet> stack;
    public void PoolSetUp(IFactory<ABullet> factory, uint initPoolSize)
    {
        this.initPoolSize = initPoolSize;
        //Stackの初期化
        stack = new Stack<ABullet>();
        //activeList = new List<APooledObject>();

        if (factory == null)
        {
            return;
        }

        //とりあえずPoolSize分instanceを生成して、見えなくしておく
        for (int i = 0; i < initPoolSize; i++)
        {
            ABullet instance = ObjectInstantiate(factory);

            instance.gameObject.transform.parent = this.transform;
            instance.gameObject.SetActive(false);
            stack.Push(instance);
        }
    }

    public ABullet GetFromPool(IFactory<ABullet> factory)
    {
        //Prefabが無ければreturn
        if (factory == null)
        {
            return null;
        }

        if (stack.Count < 1)
        {
            APooledObject newInstance = ObjectInstantiate(factory);

            newInstance.gameObject.transform.parent = this.transform;
            newInstance.pooledObjectAction += ReturnToPool;
            return newInstance;
        }

        //Stackから取り出して表示
        ABullet nextInstance = stack.Pop();
        nextInstance.gameObject.SetActive(true);
        return nextInstance;
    }

    public ABullet ObjectInstantiate(IFactory<ABullet> factory)
    {
        ABullet instance = factory.ObjectInstantiate();
        instance.pooledObjectAction += ReturnToPool;
        return instance;
    }

    public void ReturnToPool(APooledObject pooledObject)
    {
        stack.Push(pooledObject);
        pooledObject.gameObject.SetActive(false);
    }

//------------------------------------------------------------------------

    Dictionary<IFactory<ABullet> , Stack<ABullet>> poolsDic = new Dictionary<IFactory<ABullet>,  Stack<ABullet>>();

    public ABullet GetBulletFromPool(IFactory<ABullet> factory)
    {
        if (factory == null)
        {
            return null;
        }
        // もし該当の弾のプールが存在しない場合、新しく作成
        if (!poolsDic.ContainsKey(factory))
        {
            poolsDic[factory] = new Stack<ABullet>();
        }

        Stack<ABullet> bulletPool = poolsDic[factory];

        // プールに弾があればそれを使用、なければ新規作成
        if (bulletPool.Count < 1)
        {
            ABullet newInstance = ObjectInstantiate(factory);

            newInstance.gameObject.transform.parent = this.transform;
            newInstance.GetComponent<APooledObject>().pooledObjectAction += ReturnToPool;
            return newInstance;
        }

        ABullet nextInstance = bulletPool.Pop();
        nextInstance.gameObject.SetActive(true);
        return nextInstance;
    }

    // 弾をプールに戻す
    public void ReturnBullet<T>(GameObject bullet) where T : Bullet
    {
        Type bulletType = typeof(T);

        if (!poolDictionary.ContainsKey(bulletType))
        {
            poolDictionary[bulletType] = new Queue<GameObject>();
        }

        // 弾を非アクティブにしてプールに戻す
        bullet.SetActive(false);
        poolDictionary[bulletType].Enqueue(bullet);
    }
}
