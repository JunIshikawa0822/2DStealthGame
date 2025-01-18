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

    //[SerializeField]Transform referenceTransform;
    Vector3 referencePos;

    void Start()
    {
        referencePos = this.transform.position;

        Debug.Log(Convert.ToString(BitSeparateFor3D(5), 2));
        Debug.Log(Convert.ToString(BitSeparateFor3D(3)<<1, 2));
        Debug.Log(Convert.ToString(BitSeparateFor3D(4)<<2, 2));

        int cellNum = (int)Mathf.Pow(8, _dimentionLevel);
        Debug.Log("cellNum : " + cellNum);
        //#region 単位距離の計算 いらないね...
        //ルート空間における一辺あたりのマスの数を計算
        //一辺をb、マスの総数をaとすると a = b ^ 3なので両辺に1/3をかける
        //するとa ^ (1/3) = bとなる
        float baseNum = Mathf.Pow(cellNum, 1f / 3f);
        Debug.Log("baseNum : " + baseNum);
    }

    public int PosToMortonNumber(Vector3 objectPosition)
    {
        //全てのマスの総数を計算
        int cellNum = (int)Mathf.Pow(8, _dimentionLevel);

        #region 単位距離の計算 いらないね...
        //ルート空間における一辺あたりのマスの数を計算
        //一辺をb、マスの総数をaとすると a = b ^ 3なので両辺に1/3をかける
        //するとa ^ (1/3) = bとなる
        int baseNum = (int)Mathf.Pow(cellNum, 1f/3f);

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

        if(numX < 0 || numY < 0 || numZ < 0)
        {
            return -1;
        }

        return (int)GetMortonNumber((byte)numX, (byte)numY, (byte)numZ);
    }

    private uint BitSeparateFor3D(byte n)
    {
        Debug.Log("n : " + n);
        uint s = n; // 1バイトを32ビットに拡張
        Debug.Log("s : " + Convert.ToString(s, 2));

        Debug.Log("(s << 8) : " + Convert.ToString(s << 8, 2));
        Debug.Log("s | (s << 8) : " + Convert.ToString(s | (s << 8), 2));
        Debug.Log("0x0000F00F : " + Convert.ToString(0x0000F00F, 2));
        Debug.Log("(s | (s << 8)) & 0x0000F00F : " + Convert.ToString((s | (s << 8)) & 0x0000F00F , 2));
        s = (s | (s << 8)) & 0x0000F00F; // ビットを8ビット間隔で配置
        
        Debug.Log("(s << 4) : " + Convert.ToString(s << 4, 2));
        Debug.Log("s | (s << 4) : " + Convert.ToString(s | (s << 4), 2));
        Debug.Log("0x000C30C3 : " + Convert.ToString(0x000C30C3, 2));
        Debug.Log("(s | (s << 4)) & 0x000C30C3 : " + Convert.ToString((s | (s << 4)) & 0x000C30C3, 2));
        s = (s | (s << 4)) & 0x000C30C3; // ビットを4ビット間隔で配置

        Debug.Log("(s << 2) : " + Convert.ToString(s << 2, 2));
        Debug.Log("(s | (s << 2) : " + Convert.ToString(s | (s << 2), 2));
        Debug.Log("0x00249249 : " + Convert.ToString(0x00249249, 2));
        Debug.Log("(s | (s << 2)) & 0x00249249 : " + Convert.ToString((s | (s << 2)) & 0x00249249, 2));
        s = (s | (s << 2)) & 0x00249249; // ビットを2ビット間隔で配置

        Debug.Log("結果 : " + Convert.ToString(s, 2));
        //Debug.Log("最後 : " + s);
        return s;
    }

    private uint GetMortonNumber(byte x, byte y, byte z)
    {
        uint mortonNum = BitSeparateFor3D(x) | BitSeparateFor3D(y) << 1 | BitSeparateFor3D(z) << 2;

        return mortonNum;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        //全てのマスの総数を計算
        int cellNum = (int)Mathf.Pow(8, _dimentionLevel);
        //ルート空間における一辺あたりのマスの数を計算
        //一辺をb、マスの総数をaとすると a = b ^ 3なので両辺に1/3をかける
        //するとa ^ (1/3) = bとなる
        int baseNum = (int)Mathf.Pow(cellNum, 1f/3f);

        Debug.Log(baseNum);

        // XY平面
        for (int i = 0; i <= baseNum; i++)
        {
            for (int j = 0; j <= baseNum; j++)
            {
                Gizmos.color = new Color(1f, 0, 0, 0.5f);
                
                Vector3 fromOffset = (transform.right * (_cellWidth * i)) + (transform.forward * _cellDepth * j);
                Debug.Log("fromOffset : " + fromOffset);
                Vector3 toOffset = transform.up * (_cellHeight * baseNum);
                Debug.Log("toOffset : " + toOffset);

                Vector3 from = this.transform.position + fromOffset;
                //Debug.Log("from : " + from);
                Vector3 to = from + toOffset;
                //Debug.Log("to : " + to);
                
                Gizmos.DrawLine(from, to);
            }
        }

        // YZ平面
        for (int i = 0; i <= baseNum; i++)
        {
            for (int j = 0; j <= baseNum; j++)
            {
                Gizmos.color = new Color(1f, 0, 0, 0.5f);
                
                Vector3 fromOffset = (transform.up * (_cellHeight * i)) + (transform.forward * _cellDepth * j);
                Vector3 toOffset = transform.right * (_cellWidth * baseNum);

                Vector3 from = this.transform.position + fromOffset;
                Vector3 to = from + toOffset;
                
                Gizmos.DrawLine(from, to);
            }
        }

        // XZ平面
        for (int i = 0; i <= baseNum; i++)
        {
            for (int j = 0; j <= baseNum; j++)
            {
                Gizmos.color = new Color(1f, 0, 0, 0.5f);
                
                Vector3 fromOffset = (transform.right * (_cellWidth * i)) + (transform.up * _cellHeight * j);
                Vector3 toOffset = transform.forward * (_cellDepth * baseNum);

                Vector3 from = this.transform.position + fromOffset;
                Vector3 to = from + toOffset;
                
                Gizmos.DrawLine(from, to);
            }
        }
    }
}
