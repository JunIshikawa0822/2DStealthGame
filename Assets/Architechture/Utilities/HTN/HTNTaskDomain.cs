using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using TMPro;
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
