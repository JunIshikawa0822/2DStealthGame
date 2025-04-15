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
    //private AABB3DTree<(Transform, AlignedOBB)> _stageObjectTree;
    
    private HTNPlanner _planner;
    private HTNPlanRunner _planRunner;
    WorldState _worldState;

    //private EnvQuerySystem _envQuerySystem;
    private FOV _enemyFieldOfView;
    private EnvQuerySystem[] _envQuerySystems;
    private RRTSSystem _rrtsSystem;

    private Vector3 _movePosition;
    private List<Vector3> _movePaths = new List<Vector3>();

    private EnemyStatus _currentStatus = EnemyStatus.Usual;
    private bool _isShooting = false;
    private bool _isAlive = true;
    
    protected enum EnemyStatus
    {
        Usual, //普通
        Warn, //警戒
        Caution //注意
    }
    
    public override async void OnSetUp(Entity_HealthPoint enemy_Bandit_HP, AABB3DTree<(Transform, AlignedOBB)> stageObjectTree)
    {
        base.OnSetUp(enemy_Bandit_HP, stageObjectTree);

        if(EntityHP == null)
        {
            this.gameObject.SetActive(false);
            Debug.LogWarning($"{this.gameObject.name}に体力を設定してください、行動を開始できません");
            return;
        }

        //_envQuerySystem = GetComponent<EnvQuerySystem>();
        _enemyFieldOfView = GetComponent<FOV>();
        _envQuerySystems = GetComponents<EnvQuerySystem>();
        _rrtsSystem = GetComponent<RRTSSystem>();

        foreach (EnvQuerySystem query in _envQuerySystems)
        {
            query.OnSetUp(Obstacles);
        }
        
        //_envQuerySystem.OnSetUp(Obstacles);
        
        ATask rootTask = BuildTask();
        
        HTNTaskDomain domain = new HTNTaskDomain(rootTask);
        
        _planner = new HTNPlanner(domain);
        _planRunner = new HTNPlanRunner();

        Debug.Log("うごくぞ");
        //RunPlanningLoop().Forget();
    }

    public void Update()
    {
        
    }

    private async UniTask RunPlanningLoop()
    {
        Debug.Log("はじまった");
        while (_isAlive)
        {
            SearchAround();
            Rotate();
            // プランを生成
            List<ATask> plan = await _planner.GeneratePlan(_worldState);
            // Debug.Log($" プラン {plan.Count}");
            
            if (plan.Count > 0)
            {
                // プランランナー作成
                TaskStatus result = await _planRunner.StartExecution(plan, _worldState);
            
                if (result == TaskStatus.Success)
                {
                    Debug.Log("全てのタスクが正常に終了しました");
                }
                else
                {
                    Debug.LogWarning("タスク実行中にエラーが発生しました");
                }
            }
            else
            {
                Debug.LogWarning("有効なプランが生成されませんでした");
            }
        //     
        //     // 実行後に再度プランニングを行う
            await UniTask.Delay(1000 * 5); 
        }
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
                (ws) => { return true;},
                (ws) => { _isShooting = true; }
            );
        
        PrimitiveTask shotEnd = new PrimitiveTask
            (
                "ShotEnd", 
                async (ws, cts) =>
                {
                    EnemyGun.TriggerOff();
                    return TaskStatus.Success;
                },
                (ws) => { return true;},
                (ws) => { _isShooting = false;}
            );

        PrimitiveTask rotateToTarget = new PrimitiveTask
            (
                "RotateToTarget",
                async (ws, cts) =>
                {
                    
                }
            );

        Method singleFiring = new Method
        (
            "SingleFiring",
             (ws) =>
            {
                return EnemyGun != null && EnemyGun.Magazine.MagazineRemaining > 0;
            }
        );

        PrimitiveTask reload = new PrimitiveTask
            (
                "Reload",
                async(ws, cts) =>
                {
                    try
                    {
                        Debug.Log($"Reload開始");
                        await UniTask.Delay((int)(EnemyGun.ShotInterval * 1000), cancellationToken: cts);
                        Entity_Magazine magazine = new Entity_Magazine(EnemyGun.Magazine.MagazineCapacity, EnemyGun.Magazine.MagazineCapacity);
                        Debug.Log($"Reload終了");
                        return TaskStatus.Success;
                    }
                    catch(OperationCanceledException)
                    {
                        Debug.Log($"Reload中にキャンセル");
                        return TaskStatus.Canceled;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Reload中にエラー");
                        return TaskStatus.Faulted;
                    }
                },

                (ws) =>
                {
                    return EnemyGun.Magazine.MagazineRemaining < 1 && !_isShooting;
                }
            );

        PrimitiveTask findCoverPoint = new PrimitiveTask
            (
                "FindCoverPoint",
                async (ws, cts) =>
                {
                    try
                    {
                        List<Vector3> pointCandidates = _envQuerySystems[0].FindPoints();
                        // List<Vector3> pointCandidates = _envQuerySystem.FindPoints();
                        float shortestDistance = float.MaxValue;
                        List<Vector3> shortestPath = null;
                        Vector3 moveTarget = Vector3.zero;
                        
                        Vector3 startPosition = this.transform.position;

                        foreach (Vector3 target in pointCandidates)
                        {
                            List<Vector3> path =
                                _rrtsSystem.FindPath(startPosition, target, IsLineCollideWithStaticObject);

                            if (path == null || path.Count < 2) continue;
                            
                            // パスの総距離（比較用なら2乗距離で OK）
                            float totalDistance = 0f;
                            for (int i = 0; i < path.Count - 1; i++)
                            {
                                totalDistance += (path[i + 1] - path[i]).sqrMagnitude;
                            }

                            if (totalDistance < shortestDistance)
                            {
                                shortestDistance = totalDistance;
                                shortestPath = path;
                                moveTarget = target;
                            }
                        }

                        _movePosition = moveTarget;
                        _movePaths = shortestPath;

                        return TaskStatus.Success;
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.Log($"FindCoverPoint中にキャンセル");
                        return TaskStatus.Canceled;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"FindCoverPoint中にエラー {ex.Message}");
                        return TaskStatus.Faulted;
                    }
                },
                (ws) =>
                {
                    return _rrtsSystem != null && _envQuerySystems[0] != null;
                }
            );

        PrimitiveTask chasePlayer = new PrimitiveTask
        (
            "ChasePlayer",
            async (ws, cts) =>
            {
                try
                {
                    List<Vector3> pointCandidates = _envQuerySystems[1].FindPoints();
                    // List<Vector3> pointCandidates = _envQuerySystem.FindPoints();
                    float shortestDistance = float.MaxValue;
                    List<Vector3> shortestPath = null;
                    Vector3 moveTarget = Vector3.zero;
                    
                    Vector3 startPosition = this.transform.position;

                    foreach (Vector3 target in pointCandidates)
                    {
                        List<Vector3> path =
                            _rrtsSystem.FindPath(startPosition, target, IsLineCollideWithStaticObject);

                        if (path == null || path.Count < 2) continue;
                        
                        // パスの総距離（比較用なら2乗距離で OK）
                        float totalDistance = 0f;
                        for (int i = 0; i < path.Count - 1; i++)
                        {
                            totalDistance += (path[i + 1] - path[i]).sqrMagnitude;
                        }

                        if (totalDistance < shortestDistance)
                        {
                            shortestDistance = totalDistance;
                            shortestPath = path;
                            moveTarget = target;
                        }
                    }

                    _movePosition = moveTarget;
                    _movePaths = shortestPath;

                    return TaskStatus.Success;
                }
                catch (OperationCanceledException)
                {
                    Debug.Log($"ChasePlayer中にキャンセル");
                    return TaskStatus.Canceled;
                }
                catch (Exception ex)
                {
                    Debug.Log($"ChasePlayer中にエラー");
                    return TaskStatus.Faulted;
                }
            }
        );

        PrimitiveTask moveToPoint = new PrimitiveTask
        (
            "MoveToPoint",
            async (ws, cts) =>
            {
                try
                {
                    return await MoveAlongPaths(_movePaths, _actionCancellationTokenSource);
                }
                catch (Exception ex)
                {
                    Debug.Log($"MoveToPoint中にエラー");
                    return TaskStatus.Faulted;
                }
            }
        );
        
        CompoundTask chaseAndAttack = new CompoundTask("ChaseAndAttack");

        // ========= 1. 体力が少ないとき：遮蔽に隠れる =========
        Method lowHealthTakeCover = new Method("LowHealthTakeCover", ws =>
        {
            return EntityHP.CurrentHp / EntityHP.MaxHp < 0.3f; // 体力が30%未満
        });

        // 撃っていれば止める → 遮蔽物を探す → 遮蔽物に移動する
        lowHealthTakeCover.AddSubtask(shotEnd);          // 射撃終了
        lowHealthTakeCover.AddSubtask(findCoverPoint);   // 遮蔽物を探す
        lowHealthTakeCover.AddSubtask(moveToPoint);      // 遮蔽物まで移動

        chaseAndAttack.AddMethod(lowHealthTakeCover);

        // ========= 2. 通常の追跡＆射撃行動 =========
        Method chaseThenShoot = new Method("ChaseThenShoot", ws =>
        {
            return _currentTarget != null && EnemyGun != null && !_isShooting;
        });

        chaseThenShoot.AddSubtask(chasePlayer);  // プレイヤーを追跡
        chaseThenShoot.AddSubtask(shotStart);    // 射撃開始
        chaseThenShoot.AddSubtask(shotEnd);      // 射撃終了

        chaseAndAttack.AddMethod(chaseThenShoot);

        // ========= 3. 弾切れ時：リロードして攻撃再開 =========
        Method reloadThenAttack = new Method("ReloadThenAttack", ws =>
        {
            return _currentTarget != null && EnemyGun != null &&
                   EnemyGun.Magazine.MagazineRemaining <= 0 && !_isShooting;
        });

        reloadThenAttack.AddSubtask(reload);     // リロード
        reloadThenAttack.AddSubtask(chasePlayer); // 再び追跡
        reloadThenAttack.AddSubtask(shotStart);   // 射撃開始
        reloadThenAttack.AddSubtask(shotEnd);     // 射撃終了

        chaseAndAttack.AddMethod(reloadThenAttack);

        // ========= 4. ターゲットがいないとき：遮蔽物へ移動 =========
        Method findAndMoveToCover = new Method("FindAndMoveToCover", ws =>
        {
            return _currentTarget == null && !_isShooting;
        });

        findAndMoveToCover.AddSubtask(findCoverPoint); // 遮蔽物を探す
        findAndMoveToCover.AddSubtask(moveToPoint);    // 遮蔽物まで移動

        chaseAndAttack.AddMethod(findAndMoveToCover);

        return chaseAndAttack;

        return null;
    }
    
    public override void Rotate()
    {
        if(_currentTarget == null)return;
        Quaternion targetRotation = Quaternion.LookRotation(_currentTarget.position - _entityTransform.position);
        _entityTransform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(_entityTransform.eulerAngles.y, targetRotation.eulerAngles.y, _enemy_Bandit_RotateSpeed * Time.deltaTime);
    }

    private void SearchAround()
    {
        List<Transform> objectList = _enemyFieldOfView.FindTargets();
        _currentTarget = FindNearestObject(objectList, this.transform);
        
        //Debug.Log($"{this.gameObject.name} : {_currentTarget.Value.position}に敵がいるぞ！");
    }

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
        List<TreeNode3D<(Transform, AlignedOBB)>> intersectNodes = Obstacles.GetIntersectNode(lineBound);
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

    public override void OnEntityDead()
    {
        _isAlive = false;
        _actionCancellationTokenSource.Cancel();
        _actionCancellationTokenSource.Dispose();
        base.OnEntityDead();
    }
}
