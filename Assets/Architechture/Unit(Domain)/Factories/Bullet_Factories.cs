using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Factories : MonoBehaviour, IBulletFactories
{
    [SerializeField] Bullet_10mm bullet_10mm;
    [SerializeField] Bullet_5_56mm bullet_5_56mm;
    [SerializeField] Bullet_7_72mm bullet_7_72mm;
    Dictionary<Type, IFactory<ABullet>> factoriesDic;

    void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        factoriesDic = new Dictionary<Type, IFactory<ABullet>>
        {
            {typeof(IBType_10mm), new Bullet_10mm_CreateConcreteFactory(bullet_10mm)},
            {typeof(IBType_5_56mm), new Bullet_5_56mm_CreateConcreteFactory(bullet_5_56mm)},
            {typeof(IBType_7_72mm), new Bullet_7_72mm_CreateConcreteFactory(bullet_7_72mm)},
        };
    }

    public IFactory<ABullet> BulletFactory(Type type)
    {
        if(!factoriesDic.ContainsKey(type))
        {
            Debug.Log("含まれてないよ！！");
            type = typeof(IBType_10mm);
        }

        return factoriesDic[type];
    }
}
