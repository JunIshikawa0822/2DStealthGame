using UnityEngine;
[CreateAssetMenu(fileName = "Fixed_Data_Shotgun", menuName = "ScriptableObject/ItemData/Fixed/UnConsumable/Fixed_Shotgun", order = 0)]

public class Fixed_Data_Shotgun : A_Data_Fixed
{
    [SerializeField] private int simulNum;
    [SerializeField] private float spreadAngle;

    public int SimulNum{get => simulNum;}
    public float SpreadAngle{get => spreadAngle;}

    [Header("銃の基本情報")]
    [SerializeField] private I_Data_Gun.CaliberTypes _caliberType;
    [SerializeField] private uint _maxAmmoNum;
    [SerializeField] private float _shotInterval;
    [SerializeField] private float _shotVelocity;
    
    #region IGunDataの基本情報
    public I_Data_Gun.CaliberTypes CaliberType{get => _caliberType;}
    public float ShotInterval{get => _shotInterval;}
    public float ShotVelocity{get => _shotVelocity;}
    public uint MaxAmmoNum{get => _maxAmmoNum;}
    #endregion

    public override bool IsClickUse {get => false;}
    public override float UseTime{get => 0;}
}
