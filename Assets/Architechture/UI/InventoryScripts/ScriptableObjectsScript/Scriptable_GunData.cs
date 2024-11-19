using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Scriptable_GunData", menuName = "ScriptableObject/UI/ItemData/Gun", order = 0)]
public class Scriptable_GunData : ScriptableObject
{
    public string nameString;
    public bool canRotate;
    public Sprite itemImage;
    public uint widthInGUI;
    public uint heightInGUI;
    public uint stackableNum;
    public int itemID;
}
