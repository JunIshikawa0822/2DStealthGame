using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class WorldState
{
    private Dictionary<string, object> _states = new Dictionary<string, object>();
    private HashSet<string> _changedStates = new HashSet<string>();
    private event Action _onStateChanged;

    public void SetState(string key, object value)
    {
        bool changed = !_states.TryGetValue(key, out var oldValue) || !Equals(oldValue, value);
        _states[key] = value;
        
        if (changed)
        {
            _changedStates.Add(key);
            _onStateChanged?.Invoke();
        }
    }

    public T GetState<T>(string key, T defaultValue = default)
    {
        if (_states.TryGetValue(key, out object value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }
    
    public IEnumerable<string> GetAllKeys()
    {
        // 内部の_factsディクショナリからキーを返す
        return _states.Keys;
    }
    
    public WorldState Clone()
    {
        WorldState copy = new WorldState();
    
        foreach (var key in GetAllKeys())
        {
            var value = this.GetState<object>(key);
            // 値自体が参照型である場合、そのディープコピーも必要かもしれない
            copy.SetState(key, value);
        }
    
        return copy;
    }

    public bool HasFact(string key)
    {
        return _states.ContainsKey(key);
    }

    public HashSet<string> GetChangedFacts()
    {
        HashSet<string> result = new HashSet<string>(_changedStates);
        _changedStates.Clear();
        return result;
    }

    public void AddChangeListener(Action callback)
    {
        _onStateChanged += callback;
    }

    public void RemoveChangeListener(Action callback)
    {
        _onStateChanged -= callback;
    }
}

public enum TaskStatus
{
    Success,    // タスク成功
    Failure,    // タスク失敗
    Running,    // タスク実行中
    Invalid,     // タスク無効
    Canceled,   //タスクキャンセル
    Faulted     //例外発生
}
public abstract class ATask
{
    public string TaskName { get; protected set; }

    public ATask(string name)
    {
        TaskName = name;
    }
    
    //public abstract bool CheckPreCondition(WorldState worldState);

    public abstract UniTask<TaskStatus> ExecuteAsync(WorldState worldState, CancellationToken cancellationToken);
}

public class PrimitiveTask : ATask
{
    private Func<WorldState, CancellationToken, UniTask<TaskStatus>> _operatorFunc;
    private Func<WorldState, bool> _preConditionFunc;
    private Action<WorldState> _effects;
    
    public PrimitiveTask(
        string name, 
        Func<WorldState, CancellationToken, UniTask<TaskStatus>> operatorFunc,
        Func<WorldState, bool> preCondition = null,
        Action<WorldState> effects = null) : base(name)
    {
        _operatorFunc = operatorFunc;
        _preConditionFunc = preCondition ?? (_ => true);
        _effects = effects;
    }
    
    public bool CheckPreCondition(WorldState worldState)
    {
        return _preConditionFunc(worldState);
    }

    public override async UniTask<TaskStatus> ExecuteAsync(WorldState worldState, CancellationToken cancellationToken)
    {
        if (!_preConditionFunc(worldState))
        {
            return TaskStatus.Invalid;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            TaskStatus status = await _operatorFunc(worldState, cancellationToken);

            if (status == TaskStatus.Success)
            {
                _effects?.Invoke(worldState);
            }

            return status;
        }
        catch (OperationCanceledException)
        {
            // タスクがキャンセルされた場合は明示的に中断
            return TaskStatus.Canceled;
        }
        catch (Exception ex)
        {
            // 例外が発生した場合は失敗として扱い、ログに出力
            UnityEngine.Debug.LogError($"PrimitiveTask [{TaskName}] failed: {ex}");
            return TaskStatus.Failure;
        }
    }
    
    public void ApplyEffects(WorldState worldState)
    {
        _effects?.Invoke(worldState);
    }
}

public class CompoundTask : ATask
{
    private List<Method> _methods = new List<Method>();
    //private readonly Func<WorldState, bool> _preConditionFunc;

    public CompoundTask(string name) : base(name)
    {
    }

    public List<Method> GetApplicableMethods(WorldState worldState)
    {
        return _methods.Where(method => method.CheckPreCondition(worldState)).ToList();
    }

    public void AddMethod(Method method)
    {
        if (method != null)
        {
            _methods.Add(method);
        }
    }

    public override async UniTask<TaskStatus> ExecuteAsync(WorldState worldState, CancellationToken cancellationToken)
    {
        try
        {
            if (_methods.Count == 0)
            {
                Debug.LogWarning($"[CompoundTask: {TaskName}] 実行可能なメソッドが存在しません");
                return TaskStatus.Failure;
            }
            
            // キャンセルリクエストのチェック
            cancellationToken.ThrowIfCancellationRequested();
            
            Method selectedMethod = null;
            foreach (Method method in _methods)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return TaskStatus.Canceled;
                }

                if (method.CheckPreCondition(worldState))
                {
                    selectedMethod = method;
                    break;
                }
            }

            if (selectedMethod == null)
            {
                Debug.LogWarning($"[CompoundTask: {TaskName}] 実行可能なメソッドが見つかりません");
                return TaskStatus.Failure;
            }

            Debug.Log($"[CompoundTask: {TaskName}] メソッド `{selectedMethod.Name}` を選択");

            IReadOnlyList<ATask> subtasks = selectedMethod.GetSubtasks();

            for (int i = 0; i < subtasks.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.LogWarning($"[CompoundTask: {TaskName}] キャンセルされました");
                    return TaskStatus.Canceled;
                }

                ATask subtask = subtasks[i];
                Debug.Log($"[CompoundTask: {TaskName}] サブタスク `{subtask.TaskName}` を実行中...");

                TaskStatus subtaskResult = await subtask.ExecuteAsync(worldState, cancellationToken);

                if (subtaskResult != TaskStatus.Success)
                {
                    Debug.LogWarning($"[CompoundTask: {TaskName}] サブタスク `{subtask.TaskName}` が `{subtaskResult}` で終了");
                    return subtaskResult; // 失敗・キャンセル・エラー時は即終了
                }
            }

            Debug.Log($"[CompoundTask: {TaskName}] すべてのサブタスクが成功しました");
            return TaskStatus.Success;
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning($"[CompoundTask: {TaskName}] タスクがキャンセルされました");
            return TaskStatus.Canceled;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CompoundTask: {TaskName}] 実行中にエラー: {ex.Message}");
            return TaskStatus.Faulted;
        }
    }
}

public class Method
{
    public string Name { get; private set; }
    private Func<WorldState, bool> _preConditionFunc;
    private List<ATask> _subTasks = new List<ATask>();

    public Method(string name, Func<WorldState, bool> preCondition = null)
    {
        Name = name;
        _preConditionFunc = preCondition ?? (_ => true);
    }

    public void AddSubtask(ATask task)
    {
        if(task == null)return;
        
        _subTasks.Add(task);
    }

    public bool CheckPreCondition(WorldState worldState)
    {
        return _preConditionFunc(worldState);
    }

    public IReadOnlyList<ATask> GetSubtasks()
    {
        return _subTasks.AsReadOnly();
    }
}

public class HTNTaskDomain
{
    private List<ATask> _tasks = new List<ATask>();
    private ATask _rootTask;

    public HTNTaskDomain(ATask rootTask)
    {
        _rootTask = rootTask;
    }

    public void RegisterTask(ATask task)
    {
        _tasks.Add(task);
    }

    public ATask GetRootTask()
    {
        return _rootTask;
    }

    public ATask GetTaskByName(string name)
    {
        return _tasks.FirstOrDefault(t => t.TaskName == name);
    }
}

public class Planner
{
    private HTNTaskDomain _domain;
    private WorldState _worldState;
    private List<ATask> _currentPlan = new List<ATask>();

    public Planner(HTNTaskDomain domain)
    {
        _domain = domain;
        //_worldState.AddChangeListener(OnWorldStateChanged);
    }

    private void OnWorldStateChanged()
    {
        // WorldStateの変更を検知したら再プランニング
        //GeneratePlan().Forget();
    }

    public async UniTask<List<ATask>> GeneratePlan(WorldState worldState)
    {
        _currentPlan.Clear();

        // 探索を行ってプランを生成
        bool result = await DecomposeTask(_domain.GetRootTask(), worldState, _currentPlan, CancellationToken.None);

        Debug.Log($"Plan generated: {result}, Tasks: {_currentPlan.Count}");
        
        // 成功したら現在のプランを返し、失敗したら空のリストを返す
        return result ? new List<ATask>(_currentPlan) : new List<ATask>();
    }

    private async UniTask<bool> DecomposeTask(ATask task, WorldState worldState, List<ATask> plan, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return false;

        if (task is PrimitiveTask primitiveTask)
        {
            // 実行可能なPrimitiveTaskならプランに追加
            if (primitiveTask.CheckPreCondition(worldState))
            {
                plan.Add(primitiveTask);
                
                // シミュレーション用にEffectsを適用
                //もとのWorldStateをコピーして使用
                WorldState simulatedState = SimulateWorldState(worldState);
                primitiveTask.ApplyEffects(simulatedState);
                
                return true;
            }
            return false;
        }
        else if (task is CompoundTask compoundTask)
        {
            // 適用可能なMethodを探す
            List<Method> applicableMethods = compoundTask.GetApplicableMethods(worldState);
            
            if (applicableMethods.Count == 0)
                return false;

            //ここからMethod選びの戦略
            // 最初に見つかった適用可能なMethodを使用
            Method method = applicableMethods[0];
            
            // サブタスクを順に分解
            bool success = true;
            WorldState currentState = SimulateWorldState(worldState);
            
            foreach (ATask subtask in method.GetSubtasks())
            {
                success = await DecomposeTask(subtask, currentState, plan, cancellationToken);
                if (!success)
                    break;
                
                // 各サブタスクの効果を次のサブタスクに反映させる
                if (subtask is PrimitiveTask primitiveSubtask)
                {
                    primitiveSubtask.ApplyEffects(currentState);
                }
            }
            
            return success;
        }
        
        return false;
    }

    private WorldState SimulateWorldState(WorldState original)
    {
        // WorldState自身にコピー作成を任せる
        return original.Clone();
    }

    public IReadOnlyList<ATask> GetCurrentPlan()
    {
        return _currentPlan.AsReadOnly();
    }

    public void Dispose()
    {
        _worldState.RemoveChangeListener(OnWorldStateChanged);
    }
}

public class PlanRunner
{
    private bool _isExecuting = false;
    private List<ATask> _currentPlan = new List<ATask>();
    private int _currentTaskIndex = 0;
    private CancellationTokenSource _cancellationTokenSource;
    
    public async UniTask<TaskStatus> StartExecution(List<ATask> plan, WorldState worldState)
    {
        if (_isExecuting) return TaskStatus.Running;

        try
        {
            // プランを取得
            _currentPlan = plan;
            
            if (_currentPlan.Count == 0 || _currentPlan == null)
            {
                _isExecuting = false;
                return TaskStatus.Invalid;
            }

            Debug.Log($"{_currentPlan.Count}つのタスクを始めるよ");
            
            _isExecuting = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _currentTaskIndex = 0;

            // プランの実行
            while (_currentTaskIndex < _currentPlan.Count && !_cancellationTokenSource.IsCancellationRequested)
            {
                ATask currentTask = _currentPlan[_currentTaskIndex];
                Debug.Log($"次実行するタスク：{currentTask.TaskName}");

                TaskStatus status = await currentTask.ExecuteAsync(worldState, _cancellationTokenSource.Token);

                if (status == TaskStatus.Success)
                {
                    Debug.Log($"{currentTask.TaskName} の実行が成功");
                    _currentTaskIndex++; 
                }
                else if (status == TaskStatus.Failure || status == TaskStatus.Invalid || status == TaskStatus.Faulted)
                {
                    Debug.LogWarning($" {currentTask.TaskName} の実行は失敗/無効だよ");
                    return status;
                }
                else if (status == TaskStatus.Canceled)
                {
                    Debug.LogWarning($" {currentTask.TaskName} の実行はキャンセルされたよ");
                    return status;
                }
                else // Running
                {
                    // この場合はループを継続し、次のフレームで再度同じタスクを実行する
                    await UniTask.Yield();
                }
            }

            Debug.Log("全てのタスクが終了");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("プランはキャンセルされた");
        }
        catch (Exception ex)
        {
            Debug.LogError($"例外処理: {ex.Message}");
        }
        finally
        {
            //解放しようね
            _isExecuting = false;
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        
        return TaskStatus.Success;
    }

    public void StopExecution()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = null;
        _isExecuting = false;
    }

    public bool IsExecuting()
    {
        return _isExecuting;
    }
}
