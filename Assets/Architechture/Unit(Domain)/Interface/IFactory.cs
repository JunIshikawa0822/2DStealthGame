using System;
public interface IFactory<out T>
{
    public T ObjectInstantiate();

    public Type GetFactoryType();
}

