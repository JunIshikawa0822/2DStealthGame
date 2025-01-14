using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class A_Data_Customizable : I_Data_Item
{
    //カスタマイズは値段を各クラスで決めるのでPriceの設定がない
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

    public abstract int Price {get;}
    public abstract float UseTime{get;}
    public abstract bool IsClickUse{get;}
    //public abstract bool Equals(I_Data_Item data);
    #endregion

    public override bool Equals(object obj)
    {
        Debug.Log("比較");
        return Equals(obj as I_Data_Item);
    }

    public virtual bool Equals(I_Data_Item data)
    {
        Debug.Log("A_Data_Fixed");

        if(_itemName != data.ItemName) return false;
        if(_itemDiscription != data.ItemDiscription) return false;
        if(_itemImage != data.ItemImage) return false;
        if(_widthInGUI != data.Width) return false;
        if(_heightInGUI != data.Height) return false;
        if(_stackableNum != data.StackableNum) return false;
        if(_itemID != data.ItemID) return false;
        //if(_itemPrice != data.Price) return false;

        return true;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            HashCode hashCode = new HashCode();

            // プロパティに基づいてハッシュコードを生成
            hashCode.Add(_itemName);
            hashCode.Add(_itemDiscription);
            hashCode.Add(_itemID);
            hashCode.Add(_itemImage);
            hashCode.Add(_widthInGUI);
            hashCode.Add(_heightInGUI);
            hashCode.Add(_stackableNum);
            hashCode.Add(_itemID);
            //hashCode.Add(_itemPrice);

            return hashCode.ToHashCode();
        }
    }

}
