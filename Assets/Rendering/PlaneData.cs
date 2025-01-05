
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct PlaneData {
    /// <summary>
    /// 平面の法線
    /// </summary>
    public Vector3 Normal;

    /// <summary>
    /// ワールド空間上の原点からの最短距離
    /// </summary>
    public float Distance;
}

