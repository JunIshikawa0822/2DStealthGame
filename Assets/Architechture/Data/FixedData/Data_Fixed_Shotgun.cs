using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Data_Fixed_Shotgun", menuName = "ScriptableObject/ItemData/Fixed/UnConsumable/Fixed_Shotgun", order = 0)]

public class Data_Fixed_Shotgun : A_Data_Fixed<Data_Fixed_Shotgun>, I_Data_Shotgun
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

    [Header("銃の基本データ")]
    [SerializeField] float _muzzleVelocity;
    [SerializeField] float _shotInterval;
    [SerializeField] IBulletType.CaliberTypes _caliberType;
    [SerializeField] uint _maxAmmoNum;
    [SerializeField] float _reloadTime;
    [SerializeField] bool _isAuto;

    [Header("ショットガンのデータ")]
    [SerializeField] int _simulNum;
    [SerializeField] float _spreadAngle;

    #region  銃基本機能
    public IBulletType.CaliberTypes CaliberType{get => _caliberType;}
    public virtual float ShotInterval{get => _shotInterval;}
    public virtual float ShotVelocity{get => _muzzleVelocity;}
    public virtual uint MaxAmmoNum{get => _maxAmmoNum;}
    public virtual float ReloadTime{get => _reloadTime;}
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
    public virtual bool IsAuto{get => _isAuto;}
    #endregion

    #region カスタマイズかそうでないかで機能が変わるやつら
    public int Price {get => _itemPrice;}
    public float UseTime{get => 0;}
    public bool IsClickUse{get => false;}
    //public abstract bool Equals(IItemData data);
    #endregion

    #region ショットガン
    public int SimulNum{get => _simulNum;}
    public float SpreadAngle{get => _spreadAngle;}
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

            hashCode.Add(_muzzleVelocity);
            hashCode.Add(_shotInterval);
            hashCode.Add(_caliberType);
            hashCode.Add(_maxAmmoNum);
            hashCode.Add(_reloadTime);

            hashCode.Add(_simulNum);
            hashCode.Add(_spreadAngle);

            return hashCode.ToHashCode();
        }
    }

}
