using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Scriptable_Object_GunData", menuName = "ScriptableObject/Object_Data/Object_Gun/HandGun_Data", order = 0)]
public class SO_Data_HandGun : ScriptableObject, IData
{
    public string handGunName;
    public Transform handgunObjectPrefab;
    public Transform gun_Part_Grip;
    public Transform gun_Part_Magazine;
}
