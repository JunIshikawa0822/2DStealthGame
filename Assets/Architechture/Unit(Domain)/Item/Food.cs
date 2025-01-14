using System.Diagnostics;
using R3;
using UnityEngine;
public class Food : IObject
{
    float _hpHealPoint;
    float _sanityHealPoint;
    float _useTime;
    string _foodName;

    public string Name{get => _foodName;}

    public Food(I_Data_Item data)
    {
        if(data is Data_Fixed_Food foodData)
        {
            _hpHealPoint = foodData.HPHealPoint;
            _sanityHealPoint = foodData.SanityHealPoint;
            _useTime = foodData.UseTime;
            _foodName = data.ItemName;
        }
    }

    public virtual void ItemUse()
    {
        UnityEngine.Debug.Log($"{_foodName}をつかったよ");
    }
}
