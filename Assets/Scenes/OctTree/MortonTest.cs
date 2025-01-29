using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JunUtilities;
public class MortonTest : MonoBehaviour
{
    [SerializeField] Camera _camera;
    List<Vector3> _cameraCorners = new List<Vector3>();

    private float _rotationX;
    private float _rotationY;
    private float _rotationZ;

    [SerializeField] private Transform[] testObjects = new Transform[4];
    // <summary> 一番小さい空間のWidth(x軸）（何分割かはどうでもよい）</summary>
    [SerializeField] float _cellWidth;
    // <summary> 一番小さい空間のHeight(y軸）（何分割かはどうでもよい）</summary>
    [SerializeField] float _cellHeight;
    // <summary> 一番小さい空間のDepth(z軸）（何分割かはどうでもよい）</summary>
    [SerializeField] float _cellDepth;
    // <summary> 何分割するか（ルート空間は含めない）</summary>
    [SerializeField] int _dimensionLevel;
    [SerializeField] private Transform[] _objectsTrans;
    // <summary> モートン空間の基準点 </summary>
    [SerializeField] private Transform _baseTrans;
    //[SerializeField]Transform referenceTransform;
    private Vector3 _referencePos;
    void Start()
    {
        _rotationX = _camera.transform.rotation.eulerAngles.x;
        _rotationY = _camera.transform.rotation.eulerAngles.y;
        _rotationZ = _camera.transform.rotation.eulerAngles.z;
    }
    void Update()
    {
        _cameraCorners.Clear();
        Vector3[] nearCorners = JunCamera.CalculateFrustumCorners(_camera, _camera.nearClipPlane);
        //カメラのnearCornersそれぞれから、カメラの向いている方向にベクトルを伸ばす
        //地面(y座標が0)と交わる点までベクトルを伸ばし、その点がfarCorners
        for (int i = 0; i < nearCorners.Length; i++)
        {
            Vector3 direction = Vector3.forward;
            //x軸に合わせて45度回転
            JunMath.RotateAround(direction, new Vector3(_rotationX, _rotationY, _rotationZ));
            //startをS、directionをD、目的の位置をVとすると
            //Vx = Sx + tDx, Vy = Sy + tDy, Vz = Sz + tDz
            //Vy = n = 0なので、n = Sy + tDy つまり t = (n - Sy) / Dy
            float t = (float)(0 - nearCorners[i].y) / (float)direction.y; //directionVecの倍率を求める
            
            _cameraCorners.Add(nearCorners[i]);
            _cameraCorners.Add(new Vector3(nearCorners[i].x + t * direction.x, 0, nearCorners[i].z + t * direction.z));
        }
        Bounds bound = GetBounds(_cameraCorners.ToArray());
        int[] cameraMortonCodes = GetMortonCodesFromAABB(bound, new Vector3(_cellWidth, _cellHeight, _cellDepth));
        
        if(_objectsTrans.Length < 1)return;

        foreach (Transform objTrans in _objectsTrans)
        {
            
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
        bounds.SetMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        return bounds;
    }

    private int[] GetMortonCodesFromAABB(Bounds bound, Vector3 cellSize)
    {
        int separateX = Mathf.FloorToInt((bound.max.x - bound.min.x) / cellSize.x);
        int separateY = Mathf.FloorToInt((bound.max.y - bound.min.y) / cellSize.y);
        int separateZ = Mathf.FloorToInt((bound.max.z - bound.min.z) / cellSize.z);
        
        int[] intersectMortonSpaces = new int[separateX * separateY * separateZ];
        
        for (int i = 0; i < separateX; i++)
        {
            for (int j = 0; j < separateY; j++)
            {
                for (int k = 0; k < separateZ; k++)
                {
                    int num = separateX * (i + j) + separateY * j + k;
                    Vector3 pos = bound.min + new Vector3(i * cellSize.x, j * cellSize.y, k * cellSize.z);
                    int mortonNum = JunMath.PosToMortonNumber(pos, _referencePos, _dimensionLevel, cellSize);
                    intersectMortonSpaces[num] = mortonNum;
                }
            }
        }
        
        return intersectMortonSpaces;
    }
}
