using System;
public interface IPooledObject<T>
{
    public void Release();
    public void SetPoolAction(Action<T> action);
}
