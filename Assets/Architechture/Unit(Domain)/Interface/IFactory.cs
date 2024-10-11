using System;
public interface IFactory<out T>
{
    public T ObjectInstantiate(IData customData);

    public Type GetFactoryType();
}

