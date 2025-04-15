
using JunUtilities;
using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemySystem : ASystem, IOnUpdate
{
    private float _enemyRadius = 1;
    public override void OnSetUp()
    {
        gameStat.worldState = new WorldState();
        gameStat.worldState.SetState("StageCenter", gameStat.stageCenterTrans);
        gameStat.worldState.SetState("StageObjectTree", gameStat.staticObjectTree);

        // foreach (PathFinder obj in gameStat.pathFinders)
        // {
        //     if(obj == null) continue;
        //     obj.SetUp(gameStat.staticObjectTree);
        // }
        
        // if(gameStat.enemyObjects.Count < 1) return;
        // RRTStar moveAlgorithm = new RRTStar
        // (
        //     0.5f,
        //     2,
        //     _enemyRadius,
        //     IsLineCollideWithStaticObject,
        //     1000
        // );
        //
        foreach(AEnemy enemy in gameStat.enemyObjects)
        {
            if(enemy == null)continue;
            // Debug.Log(enemy.transform.name); 
            enemy.gunReleaseAction += (AGun gun) => gameStat.gunFacade.ReturnGunInstance(gun);
            enemy.OnSetUp(new Entity_HealthPoint(100, 100), gameStat.staticObjectTree);
            
            // if (enemy is Enemy_Bandit_HTN htnEnemy)
            // {
            //     htnEnemy.Initialize(gameStat.worldState);
            // }
            //enemy.onEntityDeadEvent += () => { };
            
            //enemy.SetUpEnemyAI(moveAlgorithm);
            EquipGun(enemy);
        }
    }
    public void OnUpdate()
    {
        if (AreAllItemsInactive(gameStat.enemyObjects))
        {
            gameStat.sceneLoader[1].LoadScene();
        }
    }
    
    bool AreAllItemsInactive(List<AEnemy> objects)
    {
        foreach (AEnemy obj in objects)
        {
            // 1つでもアクティブなオブジェクトがあればfalseを返す
            if (obj.gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;  // すべてのオブジェクトが非アクティブならtrue
    }
    
    public void EquipGun(AEnemy enemy)
    {
        IInventoryItem[] weaponDataArray = enemy.WeaponStorage.GetItems();
        // Debug.LogWarning(weaponDataArray[0].Data);
        if(weaponDataArray == null || weaponDataArray[0] == null)
        {
            Debug.LogWarning("武器のデータを入れてください");

            //武器のデータを入れる処理
            return;
        }

        if(!(weaponDataArray[0].Data is I_Data_Gun gunData)) return;
        
        // Debug.Log(enemy.transform.name);
        AGun enemyGun = gameStat.gunFacade.GetGunInstance(gunData).Init(gunData);
        enemyGun.ReferenceSet(weaponDataArray[0]);
        enemy.Equip(enemyGun);
        
        enemy.GetComponent<MeshChangable>().OnSetUp();
    }

    public bool IsLineCollideWithStaticObject(Vector3 startPos, Vector3 endPos)
    {
        AABB3D lineBound = new AABB3D(startPos, endPos);
        List<TreeNode3D<(Transform, AlignedOBB)>> intersectNodes = gameStat.staticObjectTree.GetIntersectNode(lineBound);

        foreach (TreeNode3D<(Transform transform, AlignedOBB allignedObb)> node in intersectNodes)
        {
            AlignedOBB obb = node.InformationTuple.allignedObb;
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
