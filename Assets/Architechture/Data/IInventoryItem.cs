using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//今はプレイヤーが自由に扱えるItemしかないけど、クエストアイテムや動かせないアイテムも作れるね
public interface IInventoryItem
{
    public I_Data_Item Data {get;}
    //public int Price {get;}
    public CellNumber Address {get; set;}
    public ItemDir Direction {get; set;}
    //public uint Width {get;}
    //public uint Height {get;}
    public uint StackingNum {get; set;}

    public enum ItemDir
    {
        Down,
        Right,
        Up,
        Left,
        Middle
    }
}
