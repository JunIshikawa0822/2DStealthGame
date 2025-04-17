using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
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
