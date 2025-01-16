using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class OctTreeBase : MonoBehaviour
{
    /// <summary>
    /// 一番小さい空間のWidth（x軸）（何分割かはどうでもよい）
    /// </summary>
    [SerializeField] float _cellWidth;

    /// <summary>
    /// 一番小さい空間のHeight(y軸）（何分割かはどうでもよい）
    /// </summary>
    [SerializeField] float _cellHeight;

    /// <summary>
    /// 一番小さい空間のDepth(z軸）（何分割かはどうでもよい）
    /// </summary>
    [SerializeField] float _cellDepth;

    /// <summary>
    /// 何分割するか（ルートは含めない）
    /// </summary>
    [SerializeField] int _dimentionLevel;

    [SerializeField]Transform referenceTransform;
    Vector3 referencePos;

    void Start()
    {
        referencePos = referenceTransform.position;
    }

    public int PosToCell(Vector3 objectPosition)
    {
        //全てのマスの総数を計算
        int cellNum = (int)Mathf.Pow(8, _dimentionLevel);

        #region 単位距離の計算 いらないね...
        //ルート空間における一辺あたりのマスの数を計算
        //一辺をb、マスの総数をaとすると a = b ^ 3なので両辺に1/3をかける
        //するとa ^ (1/3) = bとなる
        int baseNum = (int)Mathf.Pow(cellNum, 3);

        //ルート空間の一辺の長さをそれぞれ計算
        float width = _cellWidth * baseNum;
        float height = _cellHeight * baseNum;
        float depth = _cellDepth * baseNum;
        #endregion

        //cellNum個に分割された空間のx軸方向に何番目かを示しています。（indexなので0番始まりな点に注意）
        int numX = (int)((objectPosition.x - referencePos.x) / _cellWidth);

        //cellNum個に分割された空間のy軸方向に何番目かを示しています。（indexなので0番始まりな点に注意）
        int numY = (int)((objectPosition.y - referencePos.y) / _cellHeight);

        //cellNum個に分割された空間のz軸方向に何番目かを示しています。（indexなので0番始まりな点に注意）
        int numZ = (int)((objectPosition.z - referencePos.z) / _cellDepth);


    }

    private uint BitSeparateFor3D(byte n)
    {
        Debug.Log("n" + n);
        uint s = n; // 1バイトを32ビットに拡張
        Debug.Log("s" + s);

        Debug.Log("s | (s << 8)" + (s | (s << 8)));
        Debug.Log("(s | (s << 8)) & 0x0000F00F" + ((s | (s << 8)) & 0x0000F00F));
        s = (s | (s << 8)) & 0x0000F00F; // ビットを8ビット間隔で配置
        
        Debug.Log("s | (s << 4)" + (s | (s << 4)));
        Debug.Log("(s | (s << 4)) & 0x000C30C3" + ((s | (s << 4)) & 0x000C30C3));
        s = (s | (s << 4)) & 0x000C30C3; // ビットを4ビット間隔で配置

        Debug.Log("(s | (s << 2)" + (s | (s << 2)));
        Debug.Log("(s | (s << 2)) & 0x00249249" + ((s | (s << 2)) & 0x00249249));
        s = (s | (s << 2)) & 0x00249249; // ビットを2ビット間隔で配置
        return s;
    }
}
