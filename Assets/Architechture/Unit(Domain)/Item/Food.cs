using System.Diagnostics;
using R3;
using UnityEngine;
public class Food : IObject
{
    float _hpHealPoint;
    float _sanityHealPoint;
    float _useTime;
    string _foodName;

    public string Name{get;set;}

    public Food(Food_Data data)
    {
        _hpHealPoint = data.hpHealPoint;
        _sanityHealPoint = data.sanityHealPoint;
        _useTime = data.useTime;
        _foodName = data.ItemName;
        Name = data.ItemName;
    }

    public virtual void ItemUse()
    {
        UnityEngine.Debug.Log($"{_foodName}をつかったよ");
    }
}
