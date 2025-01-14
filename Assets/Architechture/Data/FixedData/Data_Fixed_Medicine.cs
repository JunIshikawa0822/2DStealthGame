
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Data_Fixed_Medicine", menuName = "ScriptableObject/ItemData/Fixed/Consumable/Fixed_Medicine", order = 0)]
public class Data_Fixed_Medicine : A_Data_Fixed<Data_Fixed_Medicine>, I_Data_Medicine
{
    [Header("GUIの情報")]
    [SerializeField]private string _itemName;
    [SerializeField]private string _itemDiscription;
    [SerializeField]private Sprite _itemImage;
    [SerializeField]private uint _widthInGUI;
    [SerializeField]private uint _heightInGUI;
    [SerializeField]private uint _stackableNum;
    [SerializeField]private int _itemID;
    [SerializeField]private int _itemPrice;

    [Header("薬の情報")]
    [SerializeField]private float _hpHealPoint;
    [SerializeField]private float _sanityHealPoint;

    [Header("使用アイテム")]
    [SerializeField]private float _useTime;

    #region GUI基本機能
    public string ItemName{get => _itemName;}
    public string ItemDiscription{get => _itemDiscription;}
    public uint Width{get => _widthInGUI;}
    public uint Height{get => _heightInGUI;}
    public uint StackableNum{get => _stackableNum;}
    public bool IsRotate{get => _widthInGUI == _heightInGUI ? false : true;}
    public Sprite ItemImage{get => _itemImage;}
    public int ItemID{get => _itemID;}
    #endregion

    #region 薬
    public float HPHealPoint{get => _hpHealPoint;}
    public float SanityHealPoint{get => _sanityHealPoint;}
    #endregion

    #region カスタマイズかそうでないかで機能が変わるやつ
    public int Price {get => _itemPrice;}
    public float UseTime{get => _useTime;}
    public bool IsClickUse{get => true;}
    //public abstract bool Equals(IItemData data);
    #endregion

    public override bool Equals(object obj)
    {
        Debug.Log("比較");
        return Equals(obj as I_Data_Item);
    }

    public bool Equals(I_Data_Item data)
    {
        if(!ReferenceEquals(this, data))return false;

        // if(_itemName != data.ItemName) return false;
        // if(_itemDiscription != data.ItemDiscription) return false;
        // if(_itemImage != data.ItemImage) return false;
        // if(_widthInGUI != data.Width) return false;
        // if(_heightInGUI != data.Height) return false;
        // if(_stackableNum != data.StackableNum) return false;
        // if(_itemID != data.ItemID) return false;
        // if(_itemPrice != data.Price) return false;

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
            hashCode.Add(_itemPrice);

            hashCode.Add(_useTime);

            hashCode.Add(_hpHealPoint);
            hashCode.Add(_sanityHealPoint);

            return hashCode.ToHashCode();
        }
    }
}
