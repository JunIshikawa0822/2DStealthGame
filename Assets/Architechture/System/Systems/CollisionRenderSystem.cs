using UnityEngine;
using System.Collections.Generic;
using JunUtilities;
using System;
using System.Linq;

public class CollisionRenderSystem : ASystem, IOnUpdate
{
    //レイマーチングコンポーネント
    private PlayerRayMarching _playerRayMarching;
    
    //静的オブジェクト管理
    private AABB3DTree _staticObjectTree;
    private Transform _staticObjectsParent;
    //動的オブジェクト管理
    //private Transform[] _dynamicObjects;
    private List<Transform> _dynamicObjectList;
     
    //カメラ
    private Camera _camera;
    private Vector3 _cameraRotateEulerAngle;
    private List<Vector3> _cameraCorners = new List<Vector3>();

    private Bounds _cameraBounds;
    private AABB3D _cameraAABB3D;
    
    //モートン空間
    private Vector3 _mortonSpaceBasePos;
    private int _dimensionLevel;
    private Vector3 _cellSize;
    
    public override void OnSetUp()
    {
        _camera = gameStat.camera;
        _staticObjectsParent = gameStat.staticObjectsParent;
        _dynamicObjectList = gameStat.dynamicObjectList;
        _mortonSpaceBasePos = gameStat.mortonSpaceBaseTrans.position;
        _dimensionLevel = gameStat.dimensionLevel;
        _cellSize = gameStat.CellSize;
        _playerRayMarching = gameStat.player.GetComponent<PlayerRayMarching>();
        
        if (_playerRayMarching == null || _camera == null || _mortonSpaceBasePos == null || _staticObjectsParent == null)
        {
            Debug.LogWarning("レイマーチングが開始できません アタッチしてください");
            return;
        }
        
        _staticObjectTree = new AABB3DTree()
            .BuildTree(GetStaticObjectList(_staticObjectsParent));
        
        _cameraRotateEulerAngle = new Vector3(
            _camera.transform.rotation.eulerAngles.x,
            _camera.transform.rotation.eulerAngles.y,
            _camera.transform.rotation.eulerAngles.z);
        
        _playerRayMarching.OnSetUp();
    }

    public void OnUpdate()
    {
        //カメラのAABB3Dをつくろう
        _cameraCorners.Clear();
        Vector3[] nearCorners = JunCamera.CalculateFrustumCorners(_camera, _camera.nearClipPlane);
        for (int i = 0; i < nearCorners.Length; i++)
        {
            Vector3 direction = Vector3.forward;
            direction = JunGeometry.RotateAround(direction, _cameraRotateEulerAngle);
            float t = (float)(0 - nearCorners[i].y) / (float)direction.y;
            
            _cameraCorners.Add(nearCorners[i]);
            _cameraCorners.Add(new Vector3(nearCorners[i].x + t * direction.x, 0, nearCorners[i].z + t * direction.z));
        }
        
        _cameraBounds = JunGeometry.GetBoundsFromVertices(_cameraCorners);
        //_cameraAABB3D = JunGeometry.GetAABB3DFromVertices(_cameraCorners);
        _cameraAABB3D = new AABB3D(_cameraBounds);
        
        _playerRayMarching.SetBounds(_cameraBounds);
        _playerRayMarching.SetAABB3D(_cameraAABB3D);
        //できた
        //_playerRayMarching.SetBound(cameraAABB3D);
        // Debug.Log(_staticObjectTree);
        //カメラが交差している可能性のあるオブジェクトたち staticに何もない場合は空のリスト
        //List<TreeNode3D> staticIntersectNodes = _staticObjectTree != null ? _staticObjectTree.GetIntersectNode(_cameraAABB3D) : new List<TreeNode3D>();
        List<TreeNode3D> staticIntersectNodes = _staticObjectTree != null ? _staticObjectTree.GetIntersectNode(_cameraAABB3D) : new List<TreeNode3D>();
        
        //カメラが交差しているモートン空間
        int[] intersectMortonSpaceNums = JunGeometry.GetMortonCodesFromAABB(_cameraAABB3D, _mortonSpaceBasePos, _dimensionLevel, _cellSize);
        HashSet<int> cameraMortonNums = new HashSet<int>(intersectMortonSpaceNums);//含まれるかの処理のためHash化
        
        //全ての動的オブジェクトに対して、カメラの交差しているモートン空間内にいるかどうかを確認、いたらリストに追加
        _dynamicObjectList = gameStat.dynamicObjectList;
        List<Transform> dynamicObjectsInCamera = new List<Transform>();
        for (int i = 0; i < _dynamicObjectList.Count; i++)
        {
            Transform dynamicTrans = _dynamicObjectList[i];
            int mortonNum = JunGeometry.PosToMortonNumber(dynamicTrans.position, _mortonSpaceBasePos, _dimensionLevel, _cellSize);

            if (cameraMortonNums.Contains(mortonNum)) { dynamicObjectsInCamera.Add(dynamicTrans); }
        }
        
        (Transform transform, int objType, OBB obb)[] objectDataArray = 
            new (Transform transform, int objType, OBB obb)[staticIntersectNodes.Count + dynamicObjectsInCamera.Count];

        //静的オブジェクト
        for (int i = 0; i < staticIntersectNodes.Count; i++)
        {
            TreeNode3D node = staticIntersectNodes[i];
            objectDataArray[i] = (node.Transform, 1, node.OrientedBounds);
        }

        //動的オブジェクト
        for (int i = 0; i < dynamicObjectsInCamera.Count; i++)
        {
            objectDataArray[staticIntersectNodes.Count + i] = (dynamicObjectsInCamera[i], 0, null);
        }
        
        //if(objectDataArray.Length < 1) return;
        _playerRayMarching.OnRayMarchingUpdate(objectDataArray);
    }

    public List<(AABB3D bounds, OBB orientedBounds, Transform transform)> GetStaticObjectList(Transform staticObjectParent)
    {
        if (staticObjectParent.childCount < 1) return null;
        
        List<(AABB3D bounds, OBB orientedBounds, Transform transform)> objectList = new List<(AABB3D bounds, OBB orientedBounds, Transform transform)>();
        MeshRenderer[] meshesArray = JunExpandUnityClass.GetChildrenComponent<MeshRenderer>(staticObjectParent);
        MeshFilter[] meshFiltersArray = JunExpandUnityClass.GetChildrenComponent<MeshFilter>(staticObjectParent);
        Transform[] transformsArray = JunExpandUnityClass.GetChildrenComponent<Transform>(staticObjectParent);
        
        for (int i = 0; i < meshesArray.Length; i++)
        {
            objectList.Add((
                new AABB3D(meshesArray[i].bounds), 
                new OBB(transformsArray[i], meshFiltersArray[i].mesh.vertices),
                transformsArray[i]
            ));
        }
        
        return objectList;
    }
}
