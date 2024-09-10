using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour, IObjectPool<APooledObject>
{
    private uint initPoolSize;

    //追加したものを、最後に追加した順に取り出せる、それ以外はListと同じ
    //private List<APooledObject> activeList;
    private Stack<APooledObject> stack;
    public void PoolSetUp(IFactory<APooledObject> factory, uint initPoolSize)
    {
        this.initPoolSize = initPoolSize;
        //Stackの初期化
        stack = new Stack<APooledObject>();
        //activeList = new List<APooledObject>();

        if (factory == null)
        {
            return;
        }

        //とりあえずPoolSize分instanceを生成して、見えなくしておく
        for (int i = 0; i < initPoolSize; i++)
        {
            APooledObject instance = ObjectInstantiate(factory);

            instance.gameObject.transform.parent = this.transform;
            instance.gameObject.SetActive(false);
            stack.Push(instance);
        }
    }

    public APooledObject GetFromPool(IFactory<APooledObject> factory)
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
        APooledObject nextInstance = stack.Pop();
        nextInstance.gameObject.SetActive(true);
        return nextInstance;
    }

    public APooledObject ObjectInstantiate(IFactory<APooledObject> factory)
    {
        APooledObject instance = factory.ObjectInstantiate();
        instance.pooledObjectAction += ReturnToPool;
        return instance;
    }

    public void ReturnToPool(APooledObject pooledObject)
    {
        stack.Push(pooledObject);
        pooledObject.gameObject.SetActive(false);
    }
}
