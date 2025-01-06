using UnityEngine;
using System;
using System.Runtime.CompilerServices;

[CreateAssetMenu(fileName = "Handgun_Data", menuName = "ScriptableObject/ItemData/UnConsumable/Gun/Handgun", order = 0)]
public class Handgun_Data : A_Item_Data, IGunData
{
    [Header("弾の種類")]
    [SerializeField] float muzzleVelocity;
    [SerializeField] float shotInterval;

    [Header("弾の種類")]
    [SerializeField] IGunData.CaliberTypes caliberType;
    [SerializeField] uint maxAmmoNum;

    //-------------------------
    public override Type DataType {get => this.GetType();}
    public IGunData.CaliberTypes CaliberType{get => caliberType;}
    public float ShotInterval{get => shotInterval;}
    public float ShotVelocity{get => muzzleVelocity;}
    public uint MaxAmmoNum{get => maxAmmoNum;}

    public bool Equals(IGunData data)
    {
        Handgun_Data hData = data as Handgun_Data;

        if(hData == null) return false;

        return 
        this.muzzleVelocity == hData.muzzleVelocity 
        &&  this.shotInterval == hData.shotInterval
        &&  this.caliberType == hData.caliberType
        &&  this.maxAmmoNum == hData.maxAmmoNum;
    }
}
