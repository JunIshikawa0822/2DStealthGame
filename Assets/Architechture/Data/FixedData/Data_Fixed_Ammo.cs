using System;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
[CreateAssetMenu(fileName = "Data_Fixed_Ammo", menuName = "ScriptableObject/ItemData/Fixed/Consumable/Fixed_Ammo", order = 0)]
public class Data_Fixed_Ammo: A_Data_Fixed<Data_Fixed_Ammo>, I_Data_Ammo
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

    [Header("弾の情報")]
    [SerializeField]private IBulletType.CaliberTypes _caliberTypes;

    #region  弾基本機能
    public IBulletType.CaliberTypes CaliberType{get => _caliberTypes;}
    #endregion

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

    #region カスタマイズかそうでないかで機能が変わるやつ
    public int Price {get => _itemPrice;}
    public float UseTime{get => 0;}
    public bool IsClickUse{get => false;}
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

            return hashCode.ToHashCode();
        }
    }
}
