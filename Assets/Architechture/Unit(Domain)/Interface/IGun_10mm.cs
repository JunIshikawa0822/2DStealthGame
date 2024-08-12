using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGun_10mm : IGun
{
    public void OnSetUp(IBulletFactories bulletFactories, IObjectPool objectPool);
    public void OnUpdate();
}
