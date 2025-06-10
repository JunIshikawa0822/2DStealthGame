using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RRTSNode
{
    public Vector3 Position;
    public RRTSNode Parent;
    public float Cost; // 開始点からのコスト

    public RRTSNode(Vector3 position, RRTSNode parent, float cost)
    {
        Position = position;
        Parent = parent;
        Cost = cost;
    }
}
