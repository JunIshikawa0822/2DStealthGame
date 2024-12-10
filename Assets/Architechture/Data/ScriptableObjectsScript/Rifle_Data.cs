using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Rifle_Data", menuName = "ScriptableObject/ItemData/UnConsumable/Gun/Rifle", order = 0)]
public class Rifle_Data : A_Item_Data, IGunData
{
    [Header("銃の情報")]
    [SerializeField] float muzzleVelocity;
    [SerializeField] float shotInterval;
    
    [Header("弾の種類")]
    [SerializeField] IGunData.CaliberTypes caliberType;
    [SerializeField] uint maxAmmoNum;
    

    //--------------------------
    public override Type DataType {get => this.GetType();}
    public IGunData.CaliberTypes CaliberType{get => caliberType;}
    public float ShotInterval{get => shotInterval;}
    public float ShotVelocity{get => muzzleVelocity;}
    public uint MaxAmmoNum{get => maxAmmoNum;}
}
