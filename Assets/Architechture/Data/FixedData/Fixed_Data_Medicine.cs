
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Fixed_Data_Medicine", menuName = "ScriptableObject/ItemData/Fixed/Consumable/Fixed_Medicine", order = 0)]
public class Fixed_Data_Medicine : A_Data_Fixed
{
    [Header("薬の情報")]
    [SerializeField]private float _hpHealPoint;
    [SerializeField]private float _sanityHealPoint;
    [SerializeField]private float _useTime;

    #region A_Data_Fixed
    public override bool IsClickUse {get => true;}
    public override float UseTime{get => _useTime;}
    #endregion

    public float hPHealPoint{get => _hpHealPoint;}
    public float SanityHealPoint{get => _sanityHealPoint;}
}
