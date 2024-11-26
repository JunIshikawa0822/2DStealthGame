using System;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class Handgun_CreateConcreteFactory : IGunFactory
{
    private GameObject _handgun;

    public Handgun_CreateConcreteFactory(GameObject handgun)
    {
        _handgun = handgun;
    }

    public IGun GunInstantiate(IGunData data)
    {
        if(data == null)return null;

        Handgun_Data gunData = data as Handgun_Data;
        if(gunData == null) return null;

        switch((int)gunData.CaliberType)
        {
            case 0 : {
                Handgun<Bullet_10mm> instance = GameObject.Instantiate(_handgun).GetComponent<Handgun<Bullet_10mm>>();
                instance.HandGunInit(gunData.ShotVelocity, gunData.ShotInterval);
                return instance;}

            case 1 : {
                Handgun<Bullet_5_56mm> instance = GameObject.Instantiate(_handgun).GetComponent<Handgun<Bullet_5_56mm>>();
                instance.HandGunInit(gunData.ShotVelocity, gunData.ShotInterval);
                return instance;}
            
            case 2 : {
                Handgun<Bullet_7_62mm> instance = GameObject.Instantiate(_handgun).GetComponent<Handgun<Bullet_7_62mm>>();
                instance.HandGunInit(gunData.ShotVelocity, gunData.ShotInterval);
                return instance;}
            
            default : {
                Handgun<Bullet_10mm> instance = GameObject.Instantiate(_handgun).GetComponent<Handgun<Bullet_10mm>>();
                instance.HandGunInit(gunData.ShotVelocity, gunData.ShotInterval);
                return instance;}
        }
    }

    // public Type GetFactoryType()
    // {
    //     return typeof(Handgun);
    // }
}
