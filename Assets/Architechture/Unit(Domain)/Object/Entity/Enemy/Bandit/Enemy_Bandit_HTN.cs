using System;
using JetBrains.Annotations;
using JunUtilities;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;

public class Enemy_Bandit_HTN : AEnemy
{
    [SerializeField] private Vector3 _enemySize;
    [SerializeField] private Transform _gunTrans;
    [SerializeField] private float _enemy_Bandit_RotateSpeed = 30;
    
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
        
        _planner = new HTNPlanner(BuildTask());
        _planRunner = new HTNPlanRunner();

        Debug.Log("うごくぞ");
        //RunPlanningLoop().Forget();
    }

    public void Update()
    {
        
    }

    private async UniTask RunPlanningLoop()
    {
        Debug.Log("プランニングループを開始しました");
        
        // 状態変化のフラグとキャンセルトークン
        bool needReplanning = true;
        CancellationTokenSource loopCts = new CancellationTokenSource();
        
        try
        {
            while (_isAlive && !loopCts.IsCancellationRequested)
            {
                // 周囲の状況確認
                SearchAround();
                
                if (needReplanning)
                {
                    Debug.Log("プランを生成中...");
                    needReplanning = false;
                    // 既存のプラン実行があれば中止
                    _planRunner?.StopExecution();
                    
                    // 新しいプランを生成
                    List<ATask> plan = await _planner.GeneratePlan(_worldState);
                    
                    if (plan.Count > 0)
                    {
                        Debug.Log($"プラン生成成功: {string.Join(" -> ", plan.Select(t => t.TaskName).ToArray())}");
                        
                        // プラン実行および
                        UniTask<TaskStatus> executionTask = _planRunner.StartExecution(plan, _worldState);
                        UniTask<bool> stateChangeTask = WaitForStateChange();
                        
                        // 実行完了を待つか、状態変化による中断を待つ
                        (int completedIndex, TaskStatus? result1, bool? result2) = await UniTask.WhenAny(executionTask, stateChangeTask);
                        
                        if (completedIndex == 0)
                        {
                            if (result1 == TaskStatus.Success)
                            {
                                Debug.Log("プランが正常に完了しました");
                                // 短い待機時間を設けて次のプランニングまで間隔を空ける
                                await UniTask.Delay(500, cancellationToken: loopCts.Token);
                            }
                            else
                            {
                                Debug.LogWarning($"プラン実行結果: {result1} - 再プランニングを行います");
                                // 失敗した場合は少し待機してから再プランニング
                                await UniTask.Delay(1000, cancellationToken: loopCts.Token);
                            }
                            
                            needReplanning = true;
                        }
                        else
                        {
                            _planRunner.StopExecution();
                            
                            // WaitForStateChangeが先に完了した場合は即再プランニング
                            Debug.Log("状態変化を検出したため再プランニングを行います");
                            needReplanning = true;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("有効なプランが生成できませんでした");
                        // プランが生成できなかった場合は待機してから再試行
                        await UniTask.Delay(2000, cancellationToken: loopCts.Token);
                    
                        // 少し待機した後にプランニング可能状態に
                        needReplanning = true;               
                    }
                }
                else
                {
                    // プランニングも実行もされていない状態（待機中）は通常ありえない
                    // 念のため状態変化を待つコードを残しておく
                    Debug.Log("状態変化を待機中...");
                    bool stateChanged = await WaitForStateChange();
                    if (stateChanged)
                    {
                        Debug.Log("状態変化を検出: 次のループで再プランニングを実行します");
                        needReplanning = true;
                    }
                }
            
                // 処理負荷軽減のための軽い待機
                await UniTask.Yield();
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("プランニングループがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogError($"プランニングループでエラーが発生: {ex}");
        }
        finally
        {
            loopCts.Dispose();
            Debug.Log("プランニングループを終了しました");
        }
    }
    
    private async UniTask<bool> WaitForStateChange()
    {
        // 実装例: 以下の条件のいずれかが変化したら再プランニングが必要
        float checkInterval = 0.2f; // 200ms間隔でチェック
    
        // while (true)
        // {
        //     await UniTask.Delay((int)(checkInterval * 1000));
        
        //     //ターゲットの状態変化をチェック
        //     if (_currentTarget != null)
        //     {
        //         return true;
        //     }
        
        //     //ターゲットのロスト
        //     if (_currentTarget == null)
        //     {
        //         return true; 
        //     }
        
        //     // 例: HPの大幅な変化
        //     float healthThreshold = 0.1f; // 10%の変化で再プランニング
        //     float currentHealth = _health;
        //     float previousHealth = _worldState.GetValue<float>("Health", currentHealth);
        
        //     if (Mathf.Abs(currentHealth - previousHealth) / _maxHealth > healthThreshold)
        //     {
        //         _worldState.SetValue("Health", currentHealth);
        //         return true;
        //     }
        // }

        return false;
    }


    public HTNTaskDomain BuildTask()
    {
        PrimitiveTask shotStartTask = new PrimitiveTask
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
        
        PrimitiveTask shotEndTask = new PrimitiveTask
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

        PrimitiveTask aimToTargetTask = new PrimitiveTask
            (
                "AimToTarget",
                async (ws, cts) =>
                    {
                        float timeout = 4f;
                        float timer = 0f;
                        
                        //目的とする方向
                        Vector3 _opponentDirection = _currentTarget.transform.position - this.transform.position;
                        while (Vector3.Angle(
                                new Vector3(this.transform.forward.x, 0, this.transform.forward.z),
                                new Vector3(_opponentDirection.x, 0, _opponentDirection.z)) > 1)
                        {
                            Rotate();
                            //更新
                            _opponentDirection = _currentTarget.transform.position - this.transform.position;
                            await UniTask.Yield(); 
                            
                            timer += Time.deltaTime;
                            if (timer > timeout) return TaskStatus.Failure;
                        }
                        
                        return TaskStatus.Success;
                    },
                    (ws) => { return true; },
                    (ws) => {}
            );

        Method singleShotMethod = new Method
        (
            "SingleShotMethod",
            (ws) =>
            {
                return EnemyGun != null && EnemyGun.Magazine.MagazineRemaining > 0;
            },
            (ws) =>
            {
                float normalizedAmmo = (float)EnemyGun.Magazine.MagazineRemaining / (float)EnemyGun.Magazine.MagazineCapacity;
                return (1 - normalizedAmmo) * (1 - normalizedAmmo);
            }
        );
        
        singleShotMethod.AddSubtask(aimToTargetTask);
        singleShotMethod.AddSubtask(shotStartTask);
        singleShotMethod.AddSubtask(shotEndTask);
        
        PrimitiveTask reloadTask = new PrimitiveTask
        (
            "Reload",
            async(ws, cts) =>
            {
                try
                {
                    Debug.Log($"Reload開始");
                    await UniTask.Delay((int)(EnemyGun.ReloadTime * 1000), cancellationToken: cts);
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
        
        Method reloadMethod = new Method
        (
            "ReloadMethod",
            (ws) =>
            {
                return EnemyGun != null && EnemyGun.Magazine.MagazineRemaining < EnemyGun.Magazine.MagazineCapacity;
            },
            (ws) =>
            {
                float normalizedAmmo = (float)EnemyGun.Magazine.MagazineRemaining / (float)EnemyGun.Magazine.MagazineCapacity;
                return (normalizedAmmo) * (normalizedAmmo);
            }
        );
        
        reloadMethod.AddSubtask(reloadTask);

        PrimitiveTask findCoverPointTask = new PrimitiveTask
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

        PrimitiveTask findOpenViewTask = new PrimitiveTask
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

        PrimitiveTask moveToPointTask = new PrimitiveTask
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
        
        Method takeCoverMethod = new Method("TakeCoverMethod");
        takeCoverMethod.AddSubtask(findCoverPointTask);
        takeCoverMethod.AddSubtask(moveToPointTask);
        
        Method takeOpenViewMethod = new Method("TakeOpenViewMethod");
        takeOpenViewMethod.AddSubtask(findOpenViewTask);
        takeOpenViewMethod.AddSubtask(moveToPointTask);

        CompoundTask combatTask = new CompoundTask("CombatTask");
        combatTask.AddMethod(singleShotMethod);
        combatTask.AddMethod(reloadMethod);
        
        CompoundTask moveTask = new CompoundTask("MoveTask");
        moveTask.AddMethod(takeCoverMethod);
        moveTask.AddMethod(takeOpenViewMethod);
        
        Method combatMethod = new Method("CombatMethod");
        combatMethod.AddSubtask(combatTask);

        Method moveMethod = new Method("MoveMethod");
        moveMethod.AddSubtask(moveTask);
        
        CompoundTask rootTask = new CompoundTask("RootTask");
        rootTask.AddMethod(combatMethod);
        rootTask.AddMethod(moveMethod);

        HTNTaskDomain taskDomain = new HTNTaskDomain(rootTask);
        
        taskDomain.RegisterTask(shotStartTask);
        taskDomain.RegisterTask(shotEndTask);
        taskDomain.RegisterTask(aimToTargetTask);
        taskDomain.RegisterTask(reloadTask);
        taskDomain.RegisterTask(findCoverPointTask);
        taskDomain.RegisterTask(findOpenViewTask);
        taskDomain.RegisterTask(moveToPointTask);
        taskDomain.RegisterTask(moveTask);
        taskDomain.RegisterTask(combatTask);
        taskDomain.RegisterTask(rootTask);
        
        return taskDomain;
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

            transform.position = target;
        
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
