using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JunUtilities;
using System.Linq;

public class StageObject : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField]PlayerRayMarching playerRayMarching;
    
    [Header("静的オブジェクトゾーン")]
    public Transform staticObjectParent;
    private AABB3DTree _stageObjectTree;
    
    [Header("動的オブジェクトゾーン")]
    public Transform dynamicObjectParent;
    private Transform[] _dynamicObjects;
    
    [SerializeField]float _cellWidth = 20;
    // <summary> 一番小さい空間のHeight(y軸）（何分割かはどうでもよい）</summary>
    [SerializeField]float _cellHeight = 20;
    // <summary> 一番小さい空間のDepth(z軸）（何分割かはどうでもよい）</summary>
    [SerializeField]float _cellDepth = 20;
    // <summary> 何分割するか（ルート空間は含めない）</summary>
    [SerializeField]int _dimensionLevel;
    // <summary> モートン空間の基準点 </summary>
    [SerializeField] private Transform _baseTrans;
    private Vector3 _cellSize;

    [Header("カメラ")]
    public Camera _camera;
    private Vector3 _rotateEulerAngle;
    List<Vector3> _cameraCorners = new List<Vector3>();
    Bounds _cameraBounds;
    AABB3D _cameraAABB3D;

    [Header("てすと")] 
    public Transform test;
    private Vector3[] _obbVertices;
    private void Start()
    {
        List<(AABB3D bounds, OBB orientedBounds, Transform transform)> objectList = new List<(AABB3D bounds, OBB orientedBounds, Transform transform)>();
        MeshRenderer[] meshesArray = JunExpandUnityClass.GetChildrenComponent<MeshRenderer>(staticObjectParent);
        MeshFilter[] meshFiltersArray = JunExpandUnityClass.GetChildrenComponent<MeshFilter>(staticObjectParent);
        Transform[] transformsArray = JunExpandUnityClass.GetChildrenComponent<Transform>(staticObjectParent);
        _dynamicObjects = JunExpandUnityClass.GetChildrenComponent<Transform>(dynamicObjectParent);

        for (int i = 0; i < meshesArray.Length; i++)
        {
            objectList.Add((
                new AABB3D(meshesArray[i].bounds), 
                new OBB(transformsArray[i], meshFiltersArray[i].mesh.vertices),
                transformsArray[i]
                ));
        }
        
        _stageObjectTree = new AABB3DTree();
        _stageObjectTree.BuildTree(objectList);
        
        // OBB obbtest = new OBB(obbTest, obbTest.GetComponent<MeshFilter>().mesh.vertices);
        // _obbTestPoints = obbtest.Vertices;
        
        _rotateEulerAngle = new Vector3(
            _camera.transform.rotation.eulerAngles.x,
            _camera.transform.rotation.eulerAngles.y,
            _camera.transform.rotation.eulerAngles.z);
        
        _cellSize = new Vector3(_cellWidth, _cellHeight, _cellDepth);
        
        playerRayMarching.OnSetUp();
        
        OBB testObb = new OBB(test, test.GetComponent<MeshFilter>().mesh.vertices);
        _obbVertices = testObb.Vertices;

    }

    private void Update()
    {
        //カメラのAABBをつくる
        _cameraCorners.Clear();
        Vector3[] nearCorners = JunCamera.CalculateFrustumCorners(_camera, _camera.nearClipPlane);
        for (int i = 0; i < nearCorners.Length; i++)
        {
            Vector3 direction = Vector3.forward;
            direction = JunGeometry.RotateAround(direction, _rotateEulerAngle);
            float t = (float)(0 - nearCorners[i].y) / (float)direction.y;
            
            _cameraCorners.Add(nearCorners[i]);
            _cameraCorners.Add(new Vector3(nearCorners[i].x + t * direction.x, 0, nearCorners[i].z + t * direction.z));
        }
        
        _cameraBounds = JunGeometry.GetBoundsFromVertices(_cameraCorners);
        _cameraAABB3D = new AABB3D(_cameraBounds);
        //_cameraAABB3D = JunGeometry.GetAABB3DFromVertices(_cameraCorners);

        //カメラのAABB
        //AABBが交差している = オブジェクトとぶつかっている　というわけではない
        List<TreeNode3D> intersectNodes = _stageObjectTree.GetIntersectNode(_cameraAABB3D);
        //List<TreeNode3D> intersectNodes = _stageObjectTree.GetIntersectNode(new AABB3D(_cameraBounds));
        
        //動的オブジェクト
        //カメラが交差しているモートン空間を取得
        int[] intersectMortonSpaceNums = JunGeometry.GetMortonCodesFromAABB(_cameraAABB3D, _baseTrans.position, _dimensionLevel, _cellSize);
        // Debug.Log(string.Join(",", intersectMortonSpaceNums));
        HashSet<int> cameraMortonNums = new HashSet<int>(intersectMortonSpaceNums);

        //全ての動的オブジェクトに対して、カメラの交差しているモートン空間内にいるかどうかを確認、いたらリストに追加
        List<Transform> dynamicObjectsInCamera = new List<Transform>();
        for (int i = 0; i < _dynamicObjects.Length; i++)
        {
            Transform dynamicTrans = _dynamicObjects[i];
            int mortonNum = JunGeometry.PosToMortonNumber(dynamicTrans.position, _baseTrans.position, _dimensionLevel, _cellSize);

            if (cameraMortonNums.Contains(mortonNum))
            {
                dynamicObjectsInCamera.Add(dynamicTrans);
            }
        }
        
        //レイマーチングに引き渡すためにオブジェクトの情報をつくる
        //まずは静的オブジェクト
        (Transform transform, int objType, OBB obb)[] objectDataArray = 
            new (Transform transform, int objType, OBB obb)[intersectNodes.Count + dynamicObjectsInCamera.Count];

        for (int i = 0; i < intersectNodes.Count; i++)
        {
            TreeNode3D node = intersectNodes[i];
            objectDataArray[i] = (node.Transform, 1, node.OrientedBounds);
        }

        for (int i = 0; i < dynamicObjectsInCamera.Count; i++)
        {
            objectDataArray[intersectNodes.Count + i] = (dynamicObjectsInCamera[i], 0, null);
        }
        
        playerRayMarching.OnRayMarchingUpdate(objectDataArray);
        // Debug.Log(
        //     string.Join((","), 
        //         intersectNodes
        //             .Where(item => item != null && item.Transform != null)
        //             .Select(item => item.Transform)));
        // Debug.Log(
        //     string.Join((","), 
        //         intersectNodes
        //             .Where(item => item != null && item.OrientedBounds != null)
        //             .Select(item => item.OrientedBounds.Center)));
    }

    public void OnDrawGizmos()
    {
        #region モートン空間表示

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

                Vector3 from = _baseTrans.position + fromOffset;
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

                Vector3 from = _baseTrans.position + fromOffset;
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

                Vector3 from = _baseTrans.position + fromOffset;
                Vector3 to = from + toOffset;
                
                Gizmos.DrawLine(from, to);
            }
        }

        #endregion
        
        if(!Application.isPlaying)return;

        #region カメラのAABB表示

        Gizmos.color = Color.green; // 緑色で描画
        Gizmos.DrawWireCube(_cameraBounds.center, _cameraBounds.size);
        //Gizmos.DrawWireCube(_cameraAABB3D.Center, _cameraAABB3D.Size);
        
        //Debug.Log($"bounds {_cameraBounds.center}, {_cameraBounds.size}");
        

        #endregion
        
        
        Gizmos.color = Color.blue;
        int[,] edges = {
            {0, 1}, {1, 3}, {3, 2}, {2, 0}, // 底面
            {4, 5}, {5, 7}, {7, 6}, {6, 4}, // 上面
            {0, 4}, {1, 5}, {2, 6}, {3, 7}  // 側面
        };
        
        for (int i = 0; i < edges.GetLength(0); i++)
        {
            Gizmos.DrawLine(_obbVertices[edges[i, 0]], _obbVertices[edges[i, 1]]);
        }
    }
}
