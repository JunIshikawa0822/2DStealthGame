using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Scriptable_MedicineData", menuName = "ScriptableObject/ItemData/Consumable/Medicine", order = 0)]
public class Scriptable_Medicine_Data : ScriptableObject, IObjectData
{
    [Header("GUIの情報")]
    [SerializeField]private string _nameString;
    [SerializeField]private bool _canRotate;
    [SerializeField]private bool _isClickUse;
    [SerializeField]private Sprite _itemImage;
    [SerializeField]private uint _widthInGUI;
    [SerializeField]private uint _heightInGUI;
    [SerializeField]private uint _stackableNum;

    [Header("薬の情報")]
    public float hpHealPoint;
    public float sanityHealPoint;
    public float useTime;
    [SerializeField]private int _itemID;

    #region GUI基本機能
    public uint Width{get => _widthInGUI;}
    public uint Height{get => _heightInGUI;}
    public uint StackableNum{get => _stackableNum;}
    public bool CanRotate{get => _canRotate;}
    public bool IsClickUse{get => _isClickUse;}
    public Sprite ItemImage{get => _itemImage;}
    public int ItemID{get => _itemID;}
    public string ItemName{get => _nameString;}
    #endregion

}
