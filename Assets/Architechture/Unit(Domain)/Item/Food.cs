using System.Diagnostics;
using R3;
using UnityEngine;
public class Food : IItem
{
    float _hpHealPoint;
    float _sanityHealPoint;
    float _useTime;
    string _foodName;

    AEntity _entity;
    public Food(Food_Data data, AEntity entity)
    {
        _hpHealPoint = data.hpHealPoint;
        _sanityHealPoint = data.sanityHealPoint;
        _useTime = data.useTime;
        _foodName = data.ItemName;

        _entity = entity;
    }

    public virtual void ItemUse()
    {
        UnityEngine.Debug.Log($"{_foodName}をつかったよ");
    }
}
