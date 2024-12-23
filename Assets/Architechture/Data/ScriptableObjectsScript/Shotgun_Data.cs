using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Shotgun_Data", menuName = "ScriptableObject/ItemData/UnConsumable/Gun/Shotgun", order = 0)]
public class Shotgun_Data : A_Item_Data, IGunData
{ 
    public int simulNum;
    public float spreadAngle;
    [Header("銃の情報")]
    [SerializeField] float muzzleVelocity;
    [SerializeField] float shotInterval;

    [Header("弾の種類")]
    [SerializeField] IGunData.CaliberTypes caliberType;
    [SerializeField] uint maxAmmoNum;

    //------------------------
    public override Type DataType {get => this.GetType();}
    public IGunData.CaliberTypes CaliberType{get => caliberType;}
    public float ShotInterval{get => shotInterval;}
    public float ShotVelocity{get => muzzleVelocity;}
    public uint MaxAmmoNum{get => maxAmmoNum;}

    public bool Equals(IGunData data)
    {
        Shotgun_Data hData = data as Shotgun_Data;

        if(hData == null) return false;

        return 
        this.muzzleVelocity == hData.muzzleVelocity 
        &&  this.shotInterval == hData.shotInterval
        &&  this.caliberType == hData.caliberType
        &&  this.maxAmmoNum == hData.maxAmmoNum;
    }
}
