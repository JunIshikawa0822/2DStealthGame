using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICategory<T>
{
    public T GetInstance(I_Data_Item data);
    public void Return (T obj);
}
