using UnityEngine;
public interface IObjectData
{
    uint StackableNum {get => count; set => count = value;}
    bool CanRotate {get; set;}
    Sprite ItemImage {get; set;}
}
