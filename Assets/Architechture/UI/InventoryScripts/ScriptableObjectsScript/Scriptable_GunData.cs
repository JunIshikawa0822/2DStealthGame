using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Scriptable_GunData", menuName = "ScriptableObject/UI/ItemData/Gun", order = 0)]
public class Scriptable_GunData : ScriptableObject, IObjectData
{
    [Header("GUIの情報")]
    [SerializeField]private string _nameString;
    [SerializeField]private bool _canRotate;
    [SerializeField]private Sprite _itemImage;
    [SerializeField]private uint _widthInGUI;
    [SerializeField]private uint _heightInGUI;
    [SerializeField]private uint _stackableNum;

    [Header("銃の情報")]
    public int simulNum;
    public float muzzleVelocity;
    public float shotInterval;
    [SerializeField]private int _itemID;

    #region GUI基本機能
    public uint Width{get => _widthInGUI;}
    public uint Height{get => _heightInGUI;}
    public uint StackableNum{get => _stackableNum;}
    public bool CanRotate{get => _canRotate;}
    public Sprite ItemImage{get => _itemImage;}
    public int ItemID{get => _itemID;}
    #endregion

}
