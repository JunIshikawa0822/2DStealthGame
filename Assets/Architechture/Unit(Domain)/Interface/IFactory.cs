using System;
public interface IFactory<out T>
{
    public T ObjectInstantiate(IObjectData customData);

    //public Type GetFactoryType();
}

