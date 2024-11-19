using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Scriptable_Object_GunData", menuName = "ScriptableObject/Object_Data/Object_Gun/HandGun_Data", order = 0)]
public class SO_Data_HandGun : ScriptableObject, IObjectData
{
    public uint StackableNum1;
    public uint StackableNum {get; set;}
    public bool CanRotate {get; set;}
    public Sprite ItemImage{get; set;}
}
