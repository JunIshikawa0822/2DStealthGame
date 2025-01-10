using System;
using UnityEngine;
public interface IFactory
{
    public IObject ObjectInstantiate(A_Item_Data data);
    public IObject ObjectInstantiate();

    public IObject ObjectInstantiate(I_Data_Item data);
    //public Type GetFactoryType();
}

