using UnityEngine;

[CreateAssetMenu(fileName = "Fixed_Food_Data", menuName = "ScriptableObject/ItemData/Fixed/Consumable/Fixed_Food", order = 0)]
public class Fixed_Food_Data : A_Data_Fixed
{
    [Header("食べ物の情報")]
    [SerializeField]private float _hpHealPoint;
    [SerializeField]private float _sanityHealPoint;
    [SerializeField]private float _useTime;

    public override bool IsClickUse {get => true;}
    public override float UseTime{get => _useTime;}

    public float hPHealPoint{get => _hpHealPoint;}
    public float SanityHealPoint{get => _sanityHealPoint;}
}
