using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;

public class HTNPlanner
{
    private HTNTaskDomain _domain;
    private WorldState _worldState;
    private List<ATask> _currentPlan = new List<ATask>();

    public HTNPlanner(HTNTaskDomain domain)
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
        
        if (task is CompoundTask compoundTask)
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
        if (original == null) return null;
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
