using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Factories : MonoBehaviour, IBulletFactories, IBType_10mm, IBType_5_56mm, IBType_7_72mm
{
    [SerializeField] Bullet_10mm bullet_10mm;
    [SerializeField] Bullet_5_56mm bullet_5_56mm;
    Dictionary<Enum, IFactory<ABullet>> factoriesDic;

    void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        factoriesDic = new Dictionary<Enum, IFactory<ABullet>>
        {
            {IBType_10mm.Caliber.Bullet_10mm, new Bullet_10mm_CreateConcreteFactory(bullet_10mm)},
            {IBType_5_56mm.Caliber.Bullet_5_56mm, new Bullet_5_56mm_CreateConcreteFactory(bullet_5_56mm)},
        };
    }

    public IFactory<ABullet> BulletFactory(Enum type)
    {
        return factoriesDic[type];
    }
}
