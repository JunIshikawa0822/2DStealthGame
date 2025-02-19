using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Data_Ammo : I_Data_Item
{
    public IBulletType.CaliberTypes CaliberType{get;}
}
