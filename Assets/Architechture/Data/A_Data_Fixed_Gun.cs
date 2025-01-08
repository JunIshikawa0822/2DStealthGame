using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class A_Data_Fixed_Gun : A_Data_Fixed
{
    public enum CaliberTypes
    {
        _10mm,
        _5_56mm,
        _7_62mm
    }

    [Header("銃の基本データ")]
    [SerializeField] float _muzzleVelocity;
    [SerializeField] float _shotInterval;
    [SerializeField] CaliberTypes _caliberType;
    [SerializeField] uint _maxAmmoNum;

    public CaliberTypes CaliberType{get => _caliberType;}
    public float ShotInterval{get => _shotInterval;}
    public float ShotVelocity{get => _muzzleVelocity;}
    public uint MaxAmmoNum{get => _maxAmmoNum;}

    public override bool Equals(I_Data_Item data)
    {
        A_Data_Fixed_Gun gunData = data as A_Data_Fixed_Gun;
        if(gunData == null)return false;

        if(_muzzleVelocity != gunData.ShotVelocity) return false;
        if(_shotInterval != gunData.ShotInterval) return false;
        if(_caliberType != gunData.CaliberType) return false;
        if(_maxAmmoNum != gunData.MaxAmmoNum) return false;

        return base.Equals(data);
    }
}
