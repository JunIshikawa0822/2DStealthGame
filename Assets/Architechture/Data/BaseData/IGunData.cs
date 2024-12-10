using UnityEngine;
public interface IGunData
{
    public enum CaliberTypes
    {
        _10mm,
        _5_56mm,
        _7_62mm
    }

    public CaliberTypes CaliberType{get;}
    public float ShotInterval{get;}
    public float ShotVelocity{get;}

    public uint MaxAmmoNum{get;}
}
