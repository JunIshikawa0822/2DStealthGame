using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace HTN
{
    public class WorldState
    {
        private Dictionary<string, object> _facts = new Dictionary<string, object>();
        private HashSet<string> _changedFacts = new HashSet<string>();
        private event Action _onStateChanged;

        public void SetFact(string key, object value)
        {
            bool changed = !_facts.TryGetValue(key, out var oldValue) || !Equals(oldValue, value);
            _facts[key] = value;
            
            if (changed)
            {
                _changedFacts.Add(key);
                _onStateChanged?.Invoke();
            }
        }

        public T GetFact<T>(string key, T defaultValue = default)
        {
            if (_facts.TryGetValue(key, out object value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        public bool HasFact(string key)
        {
            return _facts.ContainsKey(key);
        }

        public HashSet<string> GetChangedFacts()
        {
            HashSet<string> result = new HashSet<string>(_changedFacts);
            _changedFacts.Clear();
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
            
            TaskStatus status = await _operatorFunc(worldState, cancellationToken);
            
            if (status == TaskStatus.Success && _effects != null)
            {
                _effects(worldState);
            }

            return status;
        }
        
        public void ApplyEffects(WorldState worldState)
        {
            _effects?.Invoke(worldState);
        }
    }
    
    public class CompoundTask : ATask
    {
        private List<Method> _methods = new List<Method>();

        public CompoundTask(string name) : base(name)
        {
        }

        public void AddMethod(Method method)
        {
            _methods.Add(method);
        }

        public override async UniTask<TaskStatus> ExecuteAsync(WorldState worldState, CancellationToken cancellationToken)
        {
            try
            {
                // キャンセルリクエストのチェック
                cancellationToken.ThrowIfCancellationRequested();

                // 条件に合致するメソッドを探す
                foreach (var method in _methods)
                {
                    // キャンセルリクエストのチェック（ループ内でも）
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return TaskStatus.Canceled;
                    }

                    if (method.CheckPreCondition(worldState))
                    {
                        // このメソッドを選択した場合のSubTaskのリストを取得
                        return TaskStatus.Success;
                    }
                }

                // 実行可能なMethodが見つからない場合
                return TaskStatus.Failure;
            }
            catch (OperationCanceledException)
            {
                // キャンセルされた場合
                return TaskStatus.Canceled;
            }
            catch (Exception ex)
            {
                Debug.LogError($"ExecuteAsyncでエラーが発生しました: {ex.Message}");
                return TaskStatus.Faulted;
            }
        }

        public List<Method> GetApplicableMethods(WorldState worldState)
        {
            return _methods.Where(m => m.CheckPreCondition(worldState)).ToList();
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
    
    public class Domain
    {
        private List<ATask> _tasks = new List<ATask>();
        private ATask _rootTask;

        public Domain(ATask rootTask)
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
        private Domain _domain;
        private WorldState _worldState;
        private List<ATask> _currentPlan = new List<ATask>();

        public Planner(Domain domain, WorldState worldState)
        {
            _domain = domain;
            _worldState = worldState;
            _worldState.AddChangeListener(OnWorldStateChanged);
        }

        private void OnWorldStateChanged()
        {
            // WorldStateの変更を検知したら再プランニング
            GeneratePlan().Forget();
        }

        public async UniTask<bool> GeneratePlan()
        {
            _currentPlan.Clear();

            // 探索を行ってプランを生成
            bool result = await DecomposeTask(_domain.GetRootTask(), _worldState, _currentPlan, CancellationToken.None);

            Debug.Log($"Plan generated: {result}, Tasks: {_currentPlan.Count}");
            return result;
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
                    WorldState simulatedState = SimulateWorldState(worldState);
                    primitiveTask.ApplyEffects(simulatedState);
                    
                    return true;
                }
                return false;
            }
            else if (task is CompoundTask compoundTask)
            {
                // 適用可能なMethodを探す
                var applicableMethods = compoundTask.GetApplicableMethods(worldState);
                
                if (applicableMethods.Count == 0)
                    return false;

                // 最初に見つかった適用可能なMethodを使用
                var method = applicableMethods[0];
                
                // サブタスクを順に分解
                bool success = true;
                WorldState currentState = SimulateWorldState(worldState);
                
                foreach (var subtask in method.GetSubtasks())
                {
                    success = await DecomposeTask(subtask, currentState, plan, cancellationToken);
                    if (!success)
                        break;
                }
                
                return success;
            }
            
            return false;
        }

        private WorldState SimulateWorldState(WorldState original)
        {
            // 現在のWorldStateのコピーを作成
            WorldState copy = new WorldState();

            #region ディープコピーが必要
            
            foreach (var key in GetAllKeys(original))
            {
                var value = original.GetFact<object>(key);
                copy.SetFact(key, value);
            }
            
            #endregion
            return copy;
        }

        private IEnumerable<string> GetAllKeys(WorldState state)
        {
            // 注: この実装は単純化されており、実際のWorldStateでは
            // すべてのキーを取得する方法を提供する必要があります
            yield break;
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
        private Planner _planner;
        private bool _isExecuting = false;
        private List<ATask> _currentPlan = new List<ATask>();
        private int _currentTaskIndex = 0;
        private CancellationTokenSource _cancellationTokenSource;
        public void Initialize(Planner planner)
        {
            _planner = planner;
        }
        
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
            if (_isExecuting && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                _isExecuting = false;
            }
        }

        public bool IsExecuting()
        {
            return _isExecuting;
        }
    }
}

public class HTNPlanner
{
    private CompoundTask _rootTask;
    
    public HTNPlanner(CompoundTask rootTask)
    {
        _rootTask = rootTask;
    }
    
    public List<PrimitiveTask> FindPlan(WorldState worldState)
    {
        return DecomposeTask(_rootTask, worldState.Clone());
    }
    
    public void ExecutePlan(List<PrimitiveTask> plan, WorldState worldState)
    {
        foreach (PrimitiveTask task in plan)
        {
            task.Operator(worldState);
        }
    }
    
    private List<PrimitiveTask> DecomposeTask(HTNTask task, WorldState state)
    {
        if (task is PrimitiveTask primitiveTask)
        {
            return primitiveTask.PreCondition(state) ? new List<PrimitiveTask> { primitiveTask } : new List<PrimitiveTask>();
        }
        
        if (task is CompoundTask compoundTask)
        {
            foreach (Method method in compoundTask.GetValidMethods(state))
            {
                List<PrimitiveTask> plan = new List<PrimitiveTask>();
                WorldState simulatedState = state.Clone();
                
                if (DecomposeMethod(method, simulatedState, plan))
                {
                    return plan;
                }
            }
        }
        return new List<PrimitiveTask>();
    }

    private bool DecomposeMethod(Method method, WorldState state, List<PrimitiveTask> plan)
    {
        foreach (HTNTask task in method.SubTasks)
        {
            List<PrimitiveTask> subPlan = DecomposeTask(task, state);
            if (subPlan == null || subPlan.Count == 0) return false;
            plan.AddRange(subPlan);
            
            foreach (PrimitiveTask subTask in subPlan)
            {
                state = subTask.Effects(state);
            }
        }
        return true;
    }
}

public class WorldState
{
    private Dictionary<string, object> _stateVariables = new Dictionary<string, object>();
    public IAgent CurrentAgent { get; set; }
    
    public T GetValue<T>(string key)
    {
        return _stateVariables.ContainsKey(key) ? (T)_stateVariables[key] : default(T);
    }
    
    public void SetValue<T>(string key, T value)
    {
        _stateVariables[key] = value;
    }
    
    //ディープコピー
    public WorldState Clone()
    {
        WorldState clone = new WorldState();
        foreach (KeyValuePair<string, object> pair in _stateVariables)
        {
            // 基本的なディープコピー対応（必要に応じて拡張）
            if (pair.Value is ICloneable cloneable)
            {
                clone._stateVariables[pair.Key] = cloneable.Clone();
            }
            else
            {
                clone._stateVariables[pair.Key] = pair.Value;
            }
        }
        return clone;
    }
}

public interface IAgent
{
    public void SetAgentproperty();
    public T GetProperty<T>(string key, T defaultValue = default);
    public Dictionary<string, object> GetAgentProperties();
    public IAgent Clone();
}

public abstract class HTNTask
{
    protected string Name{ get; set;}
    public abstract bool PreCondition(WorldState worldState);
}

public class PrimitiveTask : HTNTask
{
    private Func<WorldState, bool> _preConditionFunc;//条件
    private Action<WorldState> _operatorAction;
    private Func<WorldState, WorldState> _effectsFunc;

    public PrimitiveTask(string name, Func<WorldState, bool> preconditionFunc, Action<WorldState> operatorAction, Func<WorldState, WorldState> effectsFunc)
     {
        Name = name;
        _preConditionFunc = preconditionFunc;
        _operatorAction = operatorAction;
        _effectsFunc = effectsFunc;
    }

    public override bool PreCondition(WorldState state)
    {
        if (_preConditionFunc == null) return false;
        return _preConditionFunc(state);
    }

    public void Operator(WorldState state)
    {
        _operatorAction?.Invoke(state);
    }

    /// <summary>
    /// 値変更の結果を返す
    /// </summary>
    /// <param name="state">シミュレーション時にはWorldのコピーを渡す</param>
    /// <returns></returns>
    public WorldState Effects(WorldState state)
    {
        if (_preConditionFunc == null) return state;
        return _effectsFunc(state);
    }
}

public class CompoundTask : HTNTask
{
    private List<Method> _methods = new List<Method>();

    public override bool PreCondition(WorldState state)
    {
        //compoundTaskの実行条件は、いずれか一つのmethodが実行可能なこと
        return _methods.Any(method => method.CheckPreconditions(state));
    }

    public List<Method> GetValidMethods(WorldState state)
    {
        return _methods.Where(method => method.CheckPreconditions(state)).ToList();
    }

    public void AddMethod(Method method)
    {
        _methods.Add(method);
    }
}

public class Method
{
    private Func<WorldState, bool> _preConditionFunc;
    private List<HTNTask> _subTasksList;
    public List<HTNTask> SubTasks {get => _subTasksList;}
    
    public Method(Func<WorldState, bool> preConditionFunc, List<HTNTask> subTasks)
    {
        _preConditionFunc = preConditionFunc;
        _subTasksList = subTasks;
    }

    // PreCondition をチェックして、現在の WorldState でこのMethodを使えるか確認する
    public bool CheckPreconditions(WorldState worldState)
    {
        return _preConditionFunc(worldState);
    }
}

