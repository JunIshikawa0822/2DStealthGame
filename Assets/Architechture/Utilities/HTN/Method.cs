using System.Collections.Generic;
using System;
using System.Linq;
public class Method
{
    public string Name { get; private set; }
    private Func<WorldState, bool> _preConditionFunc;
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
