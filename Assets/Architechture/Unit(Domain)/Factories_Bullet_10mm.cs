using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Factories_Bullet_10mm : MonoBehaviour, IBulletType, IBulletFactories
{
    //[SerializeField]public ABulletFactory[] factories;
    [SerializeField] Bullet_10mm bullet_10Mm;

    Dictionary<IBulletType.BulletType, IBulletFactory> factoriesDic;

    void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        factoriesDic = new Dictionary<IBulletType.BulletType, IBulletFactory>
        {
            {IBulletType.BulletType.Bullet_10mm, new CreateConcreteFactory_Bullet_10mm(bullet_10Mm)}
        };
    }

    public IBulletFactory BulletFactory(IBulletType.BulletType bulletType)
    {
        return factoriesDic[bulletType];
    }
}
