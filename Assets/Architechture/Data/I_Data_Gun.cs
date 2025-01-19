using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Data_Gun : I_Data_Item
{

    public IBulletType.CaliberTypes CaliberType{get;}
    public float ShotInterval{get;}
    public float ShotVelocity{get;}

    public uint MaxAmmoNum{get;}

    public float ReloadTime{get;}
    public bool IsAuto{get;}
}
