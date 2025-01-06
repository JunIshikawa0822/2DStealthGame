using UnityEngine;
public interface I_Data_Item
{
    #region GUI基本機能
    public string ItemName{get;}
    public string ItemDiscription{get;}
    public uint Width{get;}
    public uint Height{get;}
    public uint StackableNum{get;}
    public bool IsRotate{get;}
    public Sprite ItemImage{get;}
    public int ItemID{get;}
    #endregion

    #region カスタマイズかそうでないかで機能が変わるやつら
    public int Price {get;}
    public bool IsClickUse {get;}
    public float UseTime {get;}
    //public bool Equals(IItemData data);
    #endregion
}
