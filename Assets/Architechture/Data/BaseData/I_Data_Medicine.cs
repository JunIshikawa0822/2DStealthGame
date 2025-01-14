using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public interface I_Data_Medicine : I_Data_Item
{
    public float HPHealPoint{get;}
    public float SanityHealPoint{get;}
}
