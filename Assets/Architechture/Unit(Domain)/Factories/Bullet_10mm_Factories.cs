using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet_10mm_Factories : MonoBehaviour, IBulletFactories, IBulletType_10mm
{
    //[SerializeField]public ABulletFactory[] factories;
    [SerializeField] Bullet_10mm_Normal bullet_10mm_Normal;
    [SerializeField] Bullet_10mm_HdLpHc bullet_10mm_HdLpHc;
    [SerializeField] Bullet_10mm_LdHpHc bullet_10mm_LdHpHc;
    [SerializeField] Bullet_10mm_LdLpLc bullet_10mm_LdLpLc;

    Dictionary<int, IBulletFactory> factoriesDic;

    void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        factoriesDic = new Dictionary<int, IBulletFactory>
        {
            {(int)IBulletType_10mm.BulletType_10mm.Bullet_10mm_Normal, new Bullet_10mm_Normal_CreateConcreteFactory(bullet_10mm_Normal)},
            {(int)IBulletType_10mm.BulletType_10mm.Bullet_10mm_HdLpHc, new Bullet_10mm_HdLpHc_CreateConcreteFactory(bullet_10mm_HdLpHc)},
            {(int)IBulletType_10mm.BulletType_10mm.Bullet_10mm_LdHpHc, new Bullet_10mm_LdHpHc_CreateConcreteFactory(bullet_10mm_LdHpHc)},
            {(int)IBulletType_10mm.BulletType_10mm.Bullet_10mm_LdLpLc, new Bullet_10mm_LdLpLc_CreateConcreteFactory(bullet_10mm_LdLpLc)},
        };
    }

    public IBulletFactory BulletFactory(int indexNum)
    {
        return factoriesDic[indexNum];
    }
}
