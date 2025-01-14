using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemy : AEntity
{
    public Action<AGun> onEnemyDeadEvent;
    protected AGun _enemyGun;
    public abstract IStorage WeaponStorage{get;}
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
    public abstract void Move();
    public abstract void Rotate();
    public abstract void Attack();
    public abstract void Reload(AGun gun, Entity_Magazine magazine);
    public abstract void Hide();
    public abstract void Equip(AGun gun);
    public override void OnEntityDead()
    {
        onEnemyDeadEvent?.Invoke(_enemyGun);
        base.OnEntityDead();
    }
}
