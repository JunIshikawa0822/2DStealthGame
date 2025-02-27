
using JunUtilities;
using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemySystem : ASystem, IOnUpdate
{
    private float _enemyRadius = 0.5f;
    WorldState _worldState;
    public override void OnSetUp()
    {
        _worldState = new WorldState();
        
        if(gameStat.enemyObjects.Count < 1) return;
        RRTStar moveAlgorithm = new RRTStar
        (
            5,
            10,
            _enemyRadius,
            IsLineCollideWithStaticObject
        );
        
        foreach(AEnemy enemy in gameStat.enemyObjects)
        {
            Debug.Log(enemy);
            enemy.OnSetUp(new Entity_HealthPoint(100, 100));
            enemy.gunReleaseAction += (AGun gun) => gameStat.gunFacade.ReturnGunInstance(gun);
            //enemy.onEntityDeadEvent += () => { };
            
            enemy.SetUpEnemyAI(moveAlgorithm);
            EquipGun(enemy);
        }
    }
    public void OnUpdate()
    {
        
    }
    
    public void EquipGun(AEnemy enemy)
    {
        IInventoryItem[] weaponDataArray = enemy.WeaponStorage.GetItems();

        if(weaponDataArray == null || weaponDataArray[0] == null)
        {
            Debug.LogWarning("武器のデータを入れてください");

            //武器のデータを入れる処理
            return;
        }

        if(!(weaponDataArray[0].Data is I_Data_Gun gunData)) return;
        AGun enemyGun = gameStat.gunFacade.GetGunInstance(gunData).Init(gunData);
        enemy.Equip(enemyGun);
    }

    public bool IsLineCollideWithStaticObject(Vector3 startPos, Vector3 endPos)
    {
        AABB3D lineBound = new AABB3D(startPos, endPos);
        List<TreeNode3D<(Transform, AllignedOBB)>> intersectNodes = gameStat.staticObjectTree.GetIntersectNode(lineBound);

        foreach (TreeNode3D<(Transform transform, AllignedOBB allignedObb)> node in intersectNodes)
        {
            AllignedOBB obb = node.InformationTuple.allignedObb;
            Vector3 localStart = new Vector3(
                JunMath.VectorDot(obb.Center - startPos, obb.Axis[0]),
                JunMath.VectorDot(obb.Center - startPos, obb.Axis[1]),
                JunMath.VectorDot(obb.Center - startPos, obb.Axis[2])
            );
            
            Vector3 localEnd = new Vector3(
                JunMath.VectorDot(obb.Center - endPos, obb.Axis[0]),
                JunMath.VectorDot(obb.Center - endPos, obb.Axis[1]),
                JunMath.VectorDot(obb.Center - endPos, obb.Axis[2])
            );
            
            // OBBの半径を計算
            Vector3 halfExtents = obb.Size / 2;

            // 線分のAABBを計算
            Vector3 lineMin = Vector3.Min(localStart, localEnd);
            Vector3 lineMax = Vector3.Max(localStart, localEnd);
            
            // OBBとの衝突判定
            if (!(lineMax.x < -halfExtents.x - _enemyRadius || lineMin.x > halfExtents.x + _enemyRadius ||
                  lineMax.y < -halfExtents.y - _enemyRadius || lineMin.y > halfExtents.y + _enemyRadius ||
                  lineMax.z < -halfExtents.z - _enemyRadius || lineMin.z > halfExtents.z + _enemyRadius))
            {
                return true; // 衝突している
            }
        }

        return false;
    }
}
