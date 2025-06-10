using Cysharp.Threading.Tasks;
using System.Threading;

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
