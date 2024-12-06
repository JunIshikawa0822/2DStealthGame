
using System;
using UnityEngine;

public abstract class A_Item_Data : ScriptableObject
{
    [Header("GUIの情報")]
    [SerializeField]private string _nameString;
    [SerializeField]private bool _canRotate;
    [SerializeField]private bool _isClickUse;
    [SerializeField]private Sprite _itemImage;
    [SerializeField]private uint _widthInGUI;
    [SerializeField]private uint _heightInGUI;
    [SerializeField]private uint _stackableNum;

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

    public abstract Type DataType{get;}
    #endregion
}
