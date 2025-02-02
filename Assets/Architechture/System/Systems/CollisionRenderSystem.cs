using UnityEngine;
using System.Collections.Generic;
using JunUtilities;
using System;
using System.Linq;

public class CollisionRenderSystem : ASystem, IOnUpdate
{
    private Vector3 _referencePos;
    private Vector3 _cellSize;
    
    private List<MeshChangable> _oldTargetList = new List<MeshChangable>();
    //private List<MeshChangable> _newTargetList = new List<MeshChangable>();

    private Vector3 _rotateEulerAngle;
    List<Vector3> _cameraCorners = new List<Vector3>();

    private MeshChangable[] _targetObjectArray;
    private MortonTest _mortonTest;
    private Transform[] _testObjectsArray;
    public override void OnSetUp()
    {
        _referencePos = gameStat.mortonSpaceBaseTrans.position;
        _cellSize = new Vector3(gameStat.cellWidth, gameStat.cellHeight, gameStat.cellDepth);
        _rotateEulerAngle = new Vector3(
            gameStat.camera.transform.rotation.eulerAngles.x,
            gameStat.camera.transform.rotation.eulerAngles.y,
            gameStat.camera.transform.rotation.eulerAngles.z);

        _targetObjectArray = gameStat.targetParent.transform.GetComponentsInChildren<MeshChangable>();
        
        _mortonTest = gameStat.player.GetComponent<MortonTest>();
        _mortonTest.Init(_cellSize, gameStat.dimensionLevel);
        //_testObjectsArray = gameStat.testObject.transform.GetComponentsInChildren<Transform>();
    }
    public void OnUpdate()
    {
        //モートン空間とオブジェクトの引き渡し部分
        _cameraCorners.Clear();
        Vector3[] nearCorners = JunCamera.CalculateFrustumCorners(gameStat.camera, gameStat.camera.nearClipPlane);
        for (int i = 0; i < nearCorners.Length; i++)
        {
            Vector3 direction = Vector3.forward;
            direction = JunMath.RotateAround(direction, _rotateEulerAngle);
            float t = (float)(0 - nearCorners[i].y) / (float)direction.y;
            
            _cameraCorners.Add(nearCorners[i]);
            _cameraCorners.Add(new Vector3(nearCorners[i].x + t * direction.x, 0, nearCorners[i].z + t * direction.z));
        }
        
        Bounds bound = GetBounds(_cameraCorners.ToArray());//ここまではOK
        
        _mortonTest.SetBounds(bound);
        
        int[] cameraMortonCodes = GetMortonCodesFromAABB(bound, _cellSize);
        Debug.Log($"{string.Join(",", cameraMortonCodes)}");
        //Debug.Log($"{_targetObjectArray.Length}");
        if(_targetObjectArray.Length < 1)return;
        Debug.Log("動いている");
        List<MeshChangable>newTargetList = new List<MeshChangable>();
        
        foreach (MeshChangable objTrans in _targetObjectArray)
        {
            int objectMortonCode = JunMath.PosToMortonNumber(
                objTrans.transform.position, 
                gameStat.mortonSpaceBaseTrans.position, 
                gameStat.dimensionLevel, 
                _cellSize);
        
            if (Array.IndexOf(cameraMortonCodes, objectMortonCode) != -1)
            {
                newTargetList.Add(objTrans);
            }
        }
        
        DisplayVisibleTargets(newTargetList);
        Debug.Log($"最初 : {string.Join(", " , newTargetList.Select(t => t.gameObject.name))}");
        Debug.Log($"最初 : {string.Join(", " , _oldTargetList.Select(t => t.gameObject.name))}");
        
        UnDisplayInvisibleTargets(newTargetList, _oldTargetList);
        _oldTargetList = newTargetList;

        Debug.Log($"あと : {string.Join(", " , newTargetList.Select(t => t.gameObject.name))}");
        Debug.Log($"あと : {string.Join(", " , _oldTargetList.Select(t => t.gameObject.name))}");
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
        int separateX = Mathf.FloorToInt((bound.max.x - bound.min.x) / cellSize.x) + 1;
        int separateY = Mathf.FloorToInt((bound.max.y - bound.min.y) / cellSize.y) + 1;
        int separateZ = Mathf.FloorToInt((bound.max.z - bound.min.z) / cellSize.z) + 1;
        //Debug.Log((bound.max.y - bound.min.y));
        //Debug.Log(($"最大 : {bound.max.z} , 最小 : {bound.min.z}, 引いたら : {bound.max.z - bound.min.z} , cellDepth : {cellSize.z}")); 
        
        int[] intersectMortonSpaces = new int[separateX * separateY * separateZ];
        // Debug.Log($"{separateX}, {separateY}, {separateZ}");
        // Debug.Log(intersectMortonSpaces);
        
        for (int i = 0; i < separateX; i++)
        {
            for (int j = 0; j < separateY; j++)
            {
                for (int k = 0; k < separateZ; k++)
                {
                    int num = i * (separateY * separateZ) + j * separateZ + k;
                    // Debug.Log(num);
                    Vector3 pos = bound.min + new Vector3(i * cellSize.x, j * cellSize.y, k * cellSize.z);
                    // Debug.Log(num);
                    //_testObjectsArray[num].position = pos;
                    int mortonNum = JunMath.PosToMortonNumber(pos, _referencePos, gameStat.dimensionLevel, cellSize);
                    intersectMortonSpaces[num] = mortonNum;
                }
            }
        }
        
        HashSet<int> uniqueHashSet = new HashSet<int>(intersectMortonSpaces);
        int[] uniqueMortonArray = new int[uniqueHashSet.Count];
        uniqueHashSet.CopyTo(uniqueMortonArray);
        
        return uniqueMortonArray;
    }
    
    void DisplayVisibleTargets(List<MeshChangable> newVisibleTargets)
    {
        foreach(MeshChangable target in newVisibleTargets)
        {
            if(target == null)return;
            target.EntityMeshAble();
        }
    }
    
    void UnDisplayInvisibleTargets(List<MeshChangable> newVisibleTargets, List<MeshChangable> oldVisibleTargets)
    {
        
       // Debug.Log(string.Join(", ", oldVisibleTargets.Select(t => t.gameObject.name)));
        
        foreach (MeshChangable oldTarget in oldVisibleTargets)
        {
            bool isInclude = false;

            foreach (MeshChangable newTarget in newVisibleTargets)
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
                if(oldTarget ==null)return;
                oldTarget.EntityMeshDisable();
            }
        }
    }
}
