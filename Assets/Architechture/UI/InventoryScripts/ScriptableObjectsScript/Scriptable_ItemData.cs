using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Scriptable_ItemData", menuName = "ScriptableObject/UI/ItemData", order = 0)]
public class Scriptable_ItemData : ScriptableObject
{
    public enum ItemKind
    {
        Gun,
        Drink,
        Food,
        Weapon
    }

    public ItemKind itemKind;
    public string nameString;
    public Sprite itemImage;
    public uint widthInGUI;
    public uint heightInGUI;
    public uint stackableNum;
    //public ItemDir direction;
    public int itemID;
}
