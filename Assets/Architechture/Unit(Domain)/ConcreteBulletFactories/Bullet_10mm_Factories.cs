using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet_10mm_Factories : MonoBehaviour, IBulletType, IBulletFactories
{
    //[SerializeField]public ABulletFactory[] factories;
    [SerializeField] Bullet_10mm bullet_10mm;

    Dictionary<IBulletType.BulletType, IBulletFactory> factoriesDic;

    void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        factoriesDic = new Dictionary<IBulletType.BulletType, IBulletFactory>
        {
            {IBulletType.BulletType.Bullet_10mm, new Bullet_10mm_CreateConcreteFactory(bullet_10mm)}
        };
    }

    public IBulletFactory BulletFactory(IBulletType.BulletType bulletType)
    {
        return factoriesDic[bulletType];
    }
}
