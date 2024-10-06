using System;
public interface IPooledObject<T>
{
    public void Release();
    public void SetPoolEvent(Action<T> action);
}
