using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
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
