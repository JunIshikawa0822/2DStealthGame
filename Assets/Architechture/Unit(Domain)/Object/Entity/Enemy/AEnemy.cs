using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JunUtilities;

public abstract class AEnemy : AEntity
{
    public Action<AGun> gunReleaseAction;
    protected AGun EnemyGun { get; set; }
    public abstract IStorage WeaponStorage{get;}

    public abstract void SetUpEnemyAI(HTNPlanner enemyAI, RRTStar enemyMoveAlgorithm);

    public virtual void SetUpEnemyAI(RRTStar enemyMoveAlgorithm)
    {
        
    }
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
    public abstract void Rotate();
    public abstract void Attack();
    public abstract void Reload();
    public abstract void Equip(AGun gun);
    public override void OnEntityDead()
    {
        gunReleaseAction?.Invoke(EnemyGun);
        base.OnEntityDead();
        gameObject.SetActive(false);
    }
}
