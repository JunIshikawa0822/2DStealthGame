using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Factories : MonoBehaviour, IBulletCaliberFactories, IBulletCaliberType
{
    [SerializeField] Bullet_10mm bullet_10mm;
    [SerializeField] Bullet_5_56mm bullet_5_56mm;
    
    Dictionary<IBulletCaliberType.BulletCaliberType, IBulletFactory> factoriesDic;

    void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        factoriesDic = new Dictionary<IBulletCaliberType.BulletCaliberType, IBulletFactory>
        {
            {IBulletCaliberType.BulletCaliberType.Bullet_10mm, new Bullet_10mm_CreateConcreteFactory(bullet_10mm)},
            {IBulletCaliberType.BulletCaliberType.Bullet_5_56mm, new Bullet_5_56mm_CreateConcreteFactory(bullet_5_56mm)},
        };
    }

    public IBulletFactory BulletFactory(IBulletCaliberType.BulletCaliberType bulletType)
    {
        return factoriesDic[bulletType];
    }
}
