using System;
using UnityEngine;

public class Shotgun_CreateConcreteFactory : IGunFactory
{
    GameObject _shotgun;

    public Shotgun_CreateConcreteFactory(GameObject shotgun)
    {
        _shotgun = shotgun;
    }

    public IGun GunInstantiate(IGunData data)
    {
        if(data == null)return null;

        Shotgun_Data gunData = data as Shotgun_Data;
        if(gunData == null) return null;

        switch((int)gunData.CaliberType)
        {
            case 0 : {
                Shotgun<Bullet_10mm> instance = GameObject.Instantiate(_shotgun).GetComponent<Shotgun<Bullet_10mm>>();
                instance.ShotgunInit(gunData.simulNum, gunData.ShotVelocity, gunData.ShotInterval);
                return instance;}

            case 1 : {
                Shotgun<Bullet_5_56mm> instance = GameObject.Instantiate(_shotgun).GetComponent<Shotgun<Bullet_5_56mm>>();
                instance.ShotgunInit(gunData.simulNum, gunData.ShotVelocity, gunData.ShotInterval);
                return instance;}
            
            case 2 : {
                Shotgun<Bullet_7_62mm> instance = GameObject.Instantiate(_shotgun).GetComponent<Shotgun<Bullet_7_62mm>>();
                instance.ShotgunInit(gunData.simulNum, gunData.ShotVelocity, gunData.ShotInterval);
                return instance;}
            
            default : {
                Shotgun<Bullet_10mm> instance = GameObject.Instantiate(_shotgun).GetComponent<Shotgun<Bullet_10mm>>();
                instance.ShotgunInit(gunData.simulNum, gunData.ShotVelocity, gunData.ShotInterval);
                return instance;}
        }
    }
}
