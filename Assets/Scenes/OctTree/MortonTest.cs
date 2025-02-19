using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JunUtilities;
public class MortonTest : MonoBehaviour
{
    // <summary> 一番小さい空間のWidth(x軸）（何分割かはどうでもよい）</summary>
    float _cellWidth = 20;
    // <summary> 一番小さい空間のHeight(y軸）（何分割かはどうでもよい）</summary>
    float _cellHeight = 20;
    // <summary> 一番小さい空間のDepth(z軸）（何分割かはどうでもよい）</summary>
    float _cellDepth = 20;
    // <summary> 何分割するか（ルート空間は含めない）</summary>
    int _dimensionLevel;
    // <summary> モートン空間の基準点 </summary>
    [SerializeField] private Transform _baseTrans;
    //[SerializeField]Transform referenceTransform;
    private Vector3 _referencePos;
    private Vector3 _cellSize;
    
    private Bounds _bounds;
    void Start()
    {
        _bounds = new Bounds();
        _referencePos = _baseTrans.position;
    }
    
    public void Init(Vector3 cellSize, int dimensionLevel)
    {
        _cellWidth = cellSize.x;
        _cellHeight = cellSize.y;
        _cellDepth = cellSize.z;
        _dimensionLevel = dimensionLevel;
    }

    public void SetBounds(Bounds bounds)
    {
        _bounds = bounds;
    }
    
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green; // 緑色で描画
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        }
        
        int cellNum = (int)Mathf.Pow(8, _dimensionLevel);
        //ルート空間における一辺あたりのマスの数を計算
        //一辺をb、マスの総数をaとすると a = b ^ 3なので両辺に1/3をかける
        //するとa ^ (1/3) = bとなる
        int baseNum = (int)Mathf.Pow(cellNum, 1f/3f);
        // XY平面
        for (int i = 0; i <= baseNum; i++)
        {
            for (int j = 0; j <= baseNum; j++)
            {
                Gizmos.color = new Color(1f, 0, 0, 0.5f);
                
                Vector3 fromOffset = (Vector3.right * (_cellWidth * i)) + (Vector3.forward * _cellDepth * j);
                //Debug.Log("fromOffset : " + fromOffset);
                Vector3 toOffset = Vector3.up * (_cellHeight * baseNum);
                //Debug.Log("toOffset : " + toOffset);

                Vector3 from = _referencePos + fromOffset;
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
                
                Vector3 fromOffset = (Vector3.up * (_cellHeight * i)) + (Vector3.forward * _cellDepth * j);
                Vector3 toOffset = Vector3.right * (_cellWidth * baseNum);

                Vector3 from = _referencePos + fromOffset;
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
                
                Vector3 fromOffset = (Vector3.right * (_cellWidth * i)) + (Vector3.up * _cellHeight * j);
                Vector3 toOffset = Vector3.forward * (_cellDepth * baseNum);

                Vector3 from = _referencePos + fromOffset;
                Vector3 to = from + toOffset;
                
                Gizmos.DrawLine(from, to);
            }
        }
    }

    private Bounds GetBounds(Vector3[] points)
    {
        float minX = points[0].x;
        float minY = points[0].y;
        float minZ = points[0].z;
        
        float maxX = points[0].x;
        float maxY = points[0].y;
        float maxZ = points[0].z;

        for (int i = 0; i < points.Length; i++)
        {
            if(points[i].x < minX) minX = points[i].x;
            if(points[i].y < minY) minY = points[i].y;
            if(points[i].z < minZ) minZ = points[i].z;
            
            if(points[i].x > maxX) maxX = points[i].x;
            if(points[i].y > maxY) maxY = points[i].y;
            if(points[i].z > maxZ) maxZ = points[i].z;
        }
        
        Bounds bounds = new Bounds();
        // Debug.Log(new Vector3(minX, minY, minZ));
        // Debug.Log(new Vector3(maxX, maxY, maxZ));
        bounds.SetMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        return bounds;
    }

    private int[] GetMortonCodesFromAABB(Bounds bound, Vector3 cellSize)
    {
        int separateX = Mathf.FloorToInt((bound.max.x - bound.min.x) / cellSize.x);
        int separateY = Mathf.FloorToInt((bound.max.y - bound.min.y) / cellSize.y);
        int separateZ = Mathf.FloorToInt((bound.max.z - bound.min.z) / cellSize.z);
        
        int[] intersectMortonSpaces = new int[separateX * separateY * separateZ];
        Debug.Log($"{separateX}, {separateY}, {separateZ}");
        //Debug.Log(intersectMortonSpaces);
        
        for (int i = 0; i < separateX; i++)
        {
            for (int j = 0; j < separateY; j++)
            {
                for (int k = 0; k < separateZ; k++)
                {
                    int num = i * (separateY * separateZ) + j * separateZ + k;
                    // Debug.Log(num);
                    Vector3 pos = bound.min + new Vector3(i * cellSize.x, j * cellSize.y, k * cellSize.z);
                    int mortonNum = JunGeometry.PosToMortonNumber(pos, _referencePos, _dimensionLevel, cellSize);
                    intersectMortonSpaces[num] = mortonNum;
                }
            }
        }
        
        return intersectMortonSpaces;
    }
    
    void DisplayVisibleTargets(List<Transform> newVisibleTargets)
    {
        foreach(Transform target in newVisibleTargets)
        {
            MeshChangable meshChanger = target.GetComponent<MeshChangable>();

            if(meshChanger == null)return;
            meshChanger.EntityMeshAble();
        }
    }
    void UnDisplayInvisibleTargets(List<Transform> newVisibleTargets, List<Transform> oldVisibleTargets)
    {
        foreach (Transform oldTarget in oldVisibleTargets)
        {
            bool isInclude = false;

            foreach (Transform newTarget in newVisibleTargets)
            {
                //newにoldが含まれていればok
                if (oldTarget == newTarget)
                {
                    isInclude = true;
                    break;
                }
            }

            //含まれていないならオフ
            if (isInclude == false)
            {
                MeshChangable meshChanger = oldTarget.GetComponent<MeshChangable>();

                if(meshChanger == null)return;
                meshChanger.EntityMeshDisable();
            }
        }
    }
}
