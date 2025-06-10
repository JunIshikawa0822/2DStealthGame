using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
public class HTNPlanRunner
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
