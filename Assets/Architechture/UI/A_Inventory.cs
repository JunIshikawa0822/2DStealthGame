using System.Collections;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;

public abstract class A_Inventory : MonoBehaviour
{
    public abstract void OnSetUp(IObjectPool objectPool);
    public abstract void OpenInventory(IStorage storage);
    public abstract void CloseInventory();
}
