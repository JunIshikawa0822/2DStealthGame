using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_Data_Customizable : I_Data_Item
{
    [Header("GUIの情報")]
    private string _itemName;
    private string _itemDiscription;
    private Sprite _itemImage;
    private uint _widthInGUI;
    private uint _heightInGUI;
    private uint _stackableNum;
    private int _itemID;

    #region GUI基本機能
    public string ItemName{get => _itemName;}
    public string ItemDiscription{get => _itemDiscription;}
    public uint Width{get => _widthInGUI;}
    public uint Height{get => _heightInGUI;}
    public uint StackableNum{get => _stackableNum;}
    public bool IsRotate{get => _widthInGUI == _heightInGUI ? false : true;}
    public Sprite ItemImage{get => _itemImage;}
    public int ItemID{get => _itemID;}

    public int Price {get;}
    public abstract float UseTime{get;}
    public abstract bool IsClickUse{get;}
    public abstract bool Equals(I_Data_Item data);
    #endregion
}
