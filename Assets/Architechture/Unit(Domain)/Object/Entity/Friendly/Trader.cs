using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : AEntity
{
    [SerializeField]private NormalStorage _traderStorage;

    public override IStorage Storage{get => _traderStorage;}

    public override void OnDamage(float damage){}
}
