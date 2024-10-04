
using UnityEngine;

public class BulletEffect_ArmorPiercing : ScriptableObject, IEffect
{
    [SerializeField]
    public int piercingRate;
    public void CollideEffect(Collider collider, float damage)
    {
        Debug.Log("徹甲弾");
    }
}
