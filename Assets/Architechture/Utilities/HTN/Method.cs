using System.Collections.Generic;
using System;
using System.Linq;
public class Method
{
    public string Name { get; private set; }
    private Func<WorldState, bool> _preConditionFunc;
    private Func<WorldState, float> _costFunc;
    private List<ATask> _subTasks = new List<ATask>();
    
    public enum ScoringMode
    {
        Linear,
        InverseLinear,
        Square,
        HalfSine,
        InverseHalfSine,
        HalfSineSquared,
        InverseHalfSineSquared,
        SigmoidLike,
        InverseSigmoidLike
    }

    public Method(string name, Func<WorldState, bool> preCondition = null, Func<WorldState, float> costFunc = null)
    {
        Name = name;
        _preConditionFunc = preCondition ?? (_ => true);
        _costFunc = costFunc ?? (_ => 0.5f);
    }

    public void AddSubtask(ATask task)
    {
        if(task == null)return;
        
        _subTasks.Add(task);
    }

    public float CheckCost(WorldState worldState)
    {
        return _costFunc(worldState);
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
