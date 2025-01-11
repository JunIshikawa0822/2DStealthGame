using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public abstract class A_Data_Fixed_Gun : A_Data_Fixed, I_Data_Gun
{
    [Header("銃の基本データ")]
    [SerializeField] float _muzzleVelocity;
    [SerializeField] float _shotInterval;
    [SerializeField] I_Data_Gun.CaliberTypes _caliberType;
    [SerializeField] uint _maxAmmoNum;

    public I_Data_Gun.CaliberTypes CaliberType{get => _caliberType;}
    public float ShotInterval{get => _shotInterval;}
    public float ShotVelocity{get => _muzzleVelocity;}
    public uint MaxAmmoNum{get => _maxAmmoNum;}

    public override bool Equals(I_Data_Item data)
    {
        Debug.Log("I_Data_Gun");

        I_Data_Gun gunData = data as I_Data_Gun;
        if(gunData == null) return false;

        if(_muzzleVelocity != gunData.ShotVelocity) return false;
        if(_shotInterval != gunData.ShotInterval) return false;
        if(_caliberType != gunData.CaliberType) return false;
        if(_maxAmmoNum != gunData.MaxAmmoNum) return false;

        return base.Equals(data);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(base.GetHashCode());

            hashCode.Add(_muzzleVelocity);
            hashCode.Add(_shotInterval);
            hashCode.Add(_caliberType);
            hashCode.Add(_maxAmmoNum);

            return hashCode.ToHashCode();
        }
    }
}
