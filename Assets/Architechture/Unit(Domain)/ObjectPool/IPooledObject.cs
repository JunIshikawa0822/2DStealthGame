using System;
using UnityEngine;
public interface IPooledObject<T> where T : MonoBehaviour
{
    public void Release();
    public void SetPoolAction(Action<T> action);
}
