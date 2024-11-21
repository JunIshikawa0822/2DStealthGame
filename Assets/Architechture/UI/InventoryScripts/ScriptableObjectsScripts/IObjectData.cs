using UnityEngine;
public interface IObjectData
{
    public uint Width{get;}
    public uint Height{get;}
    uint StackableNum {get;}
    bool CanRotate {get;}
    Sprite ItemImage {get;}
    int ItemID{get;}
}
