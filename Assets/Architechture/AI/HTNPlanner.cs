using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

