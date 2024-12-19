using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemy : AEntity
{
     //[HideInInspector]
    protected Storage _enemyStorage;
    public Storage Storage{get => _enemyStorage;}
    public Transform FindNearestObject(List<Transform> objectList, Transform transform)
    {
        Transform nearestObject = null;
        float minSqrDistance = Mathf.Infinity; // 初期値は無限大

        foreach (Transform obj in objectList)
        {
            float sqrDistance = (obj.position - transform.position).sqrMagnitude; // 距離の二乗を取得
            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                nearestObject = obj;
            }
        }

        return nearestObject;
    }
}
