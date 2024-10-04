
using UnityEngine;

public class Entity_HealthPoint
{
    private readonly float _entityMaxHp;
    private float _entityCurrentHp;

    public Entity_HealthPoint(float maxHp, float currentHp)
    {
        if(maxHp <= 0)
        {
            maxHp = 1;
        }

        if(currentHp <= 0)
        {
            currentHp = 1;
        }

        if(maxHp < currentHp)
        {
            maxHp = currentHp;
        }

        _entityMaxHp = maxHp;
        _entityCurrentHp = currentHp;

        if(_entityCurrentHp <= 10)
        {
            Debug.LogWarning($"体力が低く設定されています：{_entityCurrentHp}");
        }
    }

    public float MaxHp{get{return _entityMaxHp;}} 
    public float CurrentHp{get{return _entityCurrentHp;}} 

    public void EntityDamage(float damage)
    {
        if(_entityCurrentHp > 0)
        {
            _entityCurrentHp -= damage;
        }
    }
}
