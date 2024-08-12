using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour, IObjectPool
{
    [SerializeField] private uint initPoolSize;

    public uint InitPoolSize => initPoolSize;

    //オブジェクトのPrefab
    //[SerializeField] private PooledObject objectToPool;

    //追加したものを、最後に追加した順に取り出せる、それ以外はListと同じ
    private Stack<APooledObject> stack;

    void IObjectPool.PoolSetUp(IBulletFactory factory)
    {
        if (factory == null)
        {
            return;
        }

        //Stackの初期化
        stack = new Stack<APooledObject>();

        //とりあえずPoolSize分instanceを生成して、見えなくしておく
        for (int i = 0; i < initPoolSize; i++)
        {
            APooledObject instance = factory.BulletObjectInstantiate();
            instance.gameObject.transform.parent = this.transform;

            instance.pooledObjectAction += ReturnToPool;
            instance.gameObject.SetActive(false);
            stack.Push(instance);
        }
    }

    APooledObject IObjectPool.GetFromPool(IBulletFactory factory)
    {
        //Prefabが無ければreturn
        if (factory == null)
        {
            return null;
        }

        //if the pool is not large enough, instantiate extra PooledObjects
        if (stack.Count == 0)
        {
            APooledObject newInstance = factory.BulletObjectInstantiate();
            newInstance.gameObject.transform.parent = this.transform;

            newInstance.pooledObjectAction += ReturnToPool;
            return newInstance;
        }

        //Stackから取り出して表示
        APooledObject nextInstance = stack.Pop();
        nextInstance.gameObject.SetActive(true);
        return nextInstance;
    }

    public void ReturnToPool(APooledObject pooledObject)
    {
        stack.Push(pooledObject);
        pooledObject.gameObject.SetActive(false);
    }
}
