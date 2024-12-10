using System;
public interface IFactory
{
    public IItem ObjectInstantiate(A_Item_Data data);
    public IItem ObjectInstantiate();
    //public Type GetFactoryType();
}

