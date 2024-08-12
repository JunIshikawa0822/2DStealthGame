using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public interface IBulletFactories
{
    public void SetUp();
    public IBulletFactory BulletFactory(IBulletType.BulletType bulletType);
}
