using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICustomizeFactory
{
    public IObject ObjectInstantiate(I_Data_Item data);
    //public Type GetFactoryType();
}
