using System;
using JetBrains.Annotations;
using JunUtilities;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;

public class Enemy_Bandit_HTN : AEnemy
{
    [SerializeField] private Vector3 _enemySize;
    [SerializeField] private Transform _gunTrans;
    [SerializeField] private float _enemy_Bandit_RotateSpeed;
    
    [SerializeField] private Transform goal;
    
    [SerializeField]private NormalStorage _enemyStorage;
    [SerializeField]private WeaponStorage _enemyWeaponStorage;
    public override IStorage Storage {get => _enemyStorage;}
    public override IStorage WeaponStorage {get => _enemyWeaponStorage;}

    private CancellationTokenSource _actionCancellationTokenSource;
    private Transform _currentTarget;
    private AABB3DTree<(Transform, AlignedOBB)> _stageObjectTree;

    private RRTStar _moveAlgorithm;
    Planner _planner;
    PlanRunner _planRunner;
    WorldState _worldState;
    
    
    public override void OnSetUp(Entity_HealthPoint enemy_Bandit_HP)
    {
        base.OnSetUp(enemy_Bandit_HP);

        if(EntityHP == null)
        {
            this.gameObject.SetActive(false);
            Debug.LogWarning($"{this.gameObject.name}に体力を設定してください、行動を開始できません");
            return;
        }
    }

    public void Initialize(WorldState worldState)
    {
        _worldState = worldState;

        if (_worldState.GetState<Transform>("StageCenter") != null)
        {
            _moveAlgorithm = new RRTStar
            (
                4f,
                15,
                2,
                IsLineCollideWithStaticObject,
                1000,
                0.2f,
                -100,
                100,
            
                //worldStateにStageCenterが入ってないと動かないよ
                _worldState.GetState<Transform>("StageCenter").position
            );
        }

        _stageObjectTree = _worldState.GetState<AABB3DTree<(Transform, AlignedOBB)>>("StageObjectTree");
        
        ATask domainTask = null;
        HTNTaskDomain domain = new HTNTaskDomain(domainTask);
        _planner = new Planner(domain);
        _planRunner = new PlanRunner();
        
        if(_moveAlgorithm == null)return;
        List<Vector3> paths = _moveAlgorithm.FindPath(this.transform.position, goal.position);
    }

    public ATask BuildTask()
    {
        PrimitiveTask shotStart = new PrimitiveTask
            (
                "ShotStart",
                async (ws , cts) =>
                {
                    EnemyGun.TriggerOn();
                    return TaskStatus.Success;
                }, 
                (ws) =>
                {
                    bool isHaveGun = EnemyGun != false;
                    bool isFindEnemy = _currentTarget != false;

                    return isHaveGun && isFindEnemy;
                }
            );
        
        PrimitiveTask shotEnd = new PrimitiveTask
            (
                "ShotEnd", 
                async (ws, cts) =>
                {
                    EnemyGun.TriggerOff();
                    return TaskStatus.Success;
                },
                (ws) =>
                {
                    bool isHaveGun = EnemyGun != false;

                    return isHaveGun;
                }
            );

        PrimitiveTask reload = new PrimitiveTask
            (
                "Reload",
                async(ws, cts) =>
                {
                    Entity_Magazine magazine = new Entity_Magazine(EnemyGun.Magazine.MagazineCapacity, EnemyGun.Magazine.MagazineCapacity);

                    try
                    {
                        Debug.Log($"Reload開始");
                        await UniTask.Delay((int)(EnemyGun.ShotInterval * 1000), cancellationToken: cts);
                        return TaskStatus.Success;
                    }
                    catch(OperationCanceledException)
                    {
                        return TaskStatus.Canceled;
                    }
                    catch (Exception ex)
                    {
                        return TaskStatus.Faulted;
                    }
                },

                (ws) =>
                {
                    return EnemyGun.Magazine.MagazineRemaining < 1;
                }
            );

        return null;
    }
    
    public override void Rotate()
    {
        if(_currentTarget == null)return;
        Quaternion targetRotation = Quaternion.LookRotation(_currentTarget.position - _entityTransform.position);
        _entityTransform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(_entityTransform.eulerAngles.y, targetRotation.eulerAngles.y, _enemy_Bandit_RotateSpeed * Time.deltaTime);
    }

    // public void Reload()
    // {
    //     if(EnemyGun == null)return;
    //     if(EnemyGun.Magazine == null)return;
    //     if(EnemyGun.Magazine.MagazineRemaining >= EnemyGun.Magazine.MagazineCapacity)return;
    //
    //     EnemyGun.Reload(new Entity_Magazine(EnemyGun.Magazine.MagazineCapacity, EnemyGun.Magazine.MagazineCapacity));
    // }

    public override void Equip(AGun gun)
    {
        EnemyGun = gun;
        gun.transform.SetParent(_gunTrans);
        gun.transform.SetPositionAndRotation(_gunTrans.position, this.transform.rotation);
    }
    public override void OnDamage(float damage)
    {
        EntityHP.EntityDamage(damage);
        if(IsEntityDead())
        {
            base.OnEntityDead();
        }
    }
    
    //ノードを移動する
    private async UniTask<TaskStatus> MoveAlongPaths(List<Vector3> paths, CancellationTokenSource actionCTS)
    {
        foreach (Vector3 target in paths)
        {
            await MoveToTarget(target, actionCTS);
        }

        return TaskStatus.Success;
        
        //ノードからノードへ移動
        async UniTask<TaskStatus> MoveToTarget(Vector3 target, CancellationTokenSource actionCTS)
        {
            float stuckTimeThreshold = 2.0f;
            float noMovementTimer = 0f;
            Vector3 lastPosition = transform.localPosition;
            float movementThreshold = 0.1f;
            // 目標に到達するまでループ
            while ((target - transform.position).sqrMagnitude > _enemySize.y + _enemySize.x)
            {
                Debug.Log("移動中");
                Debug.Log($"ターゲット : {target}, 現在 : {transform.position}, 距離 : {(target - transform.position).sqrMagnitude}");
            
                // 目標方向の単位ベクトルを算出し、localPositionに加算して移動
                Vector3 direction = (target - transform.position).normalized;
                transform.localPosition += direction * 5 * Time.deltaTime;

                if ((transform.localPosition - lastPosition).sqrMagnitude < movementThreshold)
                {
                    noMovementTimer += Time.deltaTime;
                    if (noMovementTimer >= stuckTimeThreshold)
                    {
                        Debug.Log("一定時間動いていないため移動を中断します。");
                        return TaskStatus.Canceled;
                    }
                }
                else
                {
                    noMovementTimer = 0f;
                    lastPosition = transform.localPosition;
                }

                // 次のフレームの Update 時に処理を再開
                await UniTask.Yield(PlayerLoopTiming.Update, actionCTS.Token);
            }
        
            Debug.Log("1フェーズ終了");
            return TaskStatus.Success;
        }
    }
    
    //RRTStarにぶち込む、接触判定
    public bool IsLineCollideWithStaticObject(Vector3 startPos, Vector3 endPos)
    {
        AABB3D lineBound = new AABB3D(startPos, endPos);
        List<TreeNode3D<(Transform, AlignedOBB)>> intersectNodes = _stageObjectTree.GetIntersectNode(lineBound);
        foreach (TreeNode3D<(Transform transform, AlignedOBB allignedObb)> node in intersectNodes)
        {
            // Debug.Log($"オブジェクト : {node.InformationTuple.transform.name}, " +
            //           $"サイズ : {node.InformationTuple.allignedObb.Size}");
            bool isCollide = node.InformationTuple.allignedObb.IsThickLineIntersection(startPos, endPos, _enemySize.x, _enemySize.y);

            if (isCollide)
            {
                return true;
            }
        }
        return false;
    }
}
