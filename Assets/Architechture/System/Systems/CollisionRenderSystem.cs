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
    //private AABB3DTree _staticObjectTree;
    private Transform _staticObjectsParent;
    
    private AABB3DTree<(Transform, AlignedOBB)> _staticObjectTree;
    //動的オブジェクト管理
    //private Transform[] _dynamicObjects;
    private List<Transform> _dynamicObjectList;
    
    //宝箱管理
    private List<Transform> _InteractableObjectList;
    
    //カメラ
    private Camera _camera;
    private Vector3 _cameraRotateEulerAngle;
    private List<Vector3> _cameraCorners = new List<Vector3>();

    //private Bounds _cameraBounds;
    private AABB3D _cameraAABB3D;
    
    //モートン空間
    private Vector3 _mortonSpaceBasePos;
    private int _dimensionLevel;
    private Vector3 _cellSize;
    
    List<Transform> _oldMeshableList = new List<Transform>();
    
    //テスト用
    private Vector3[] _obbVertices;
    
    public override void OnSetUp()
    {
        _camera = gameStat.camera;
        _staticObjectsParent = gameStat.staticObjectsParent;
        _dynamicObjectList = gameStat.dynamicObjectList;
        _InteractableObjectList = gameStat.InteractableObjects;
        _mortonSpaceBasePos = gameStat.mortonSpaceBaseTrans.position;
        _dimensionLevel = gameStat.dimensionLevel;
        _cellSize = gameStat.CellSize;
        _playerRayMarching = gameStat.player.GetComponent<PlayerRayMarching>();
        
        if (_playerRayMarching == null || _camera == null || _mortonSpaceBasePos == null || _staticObjectsParent == null)
        {
            Debug.LogWarning("レイマーチングが開始できません アタッチしてください");
            return;
        }

        List<(AABB3D bounds, Transform transform, AlignedOBB allignedObb)> originalList = GetStaticObjectList(_staticObjectsParent);
        List<(AABB3D bounds, (Transform, AlignedOBB))> convertedList = 
            originalList
            .Select(item => (item.bounds, (item.transform, item.allignedObb)))
            .ToList();
        
        _staticObjectTree = new AABB3DTree<(Transform, AlignedOBB)>()
            .BuildTree(convertedList);
        
        _cameraRotateEulerAngle = new Vector3(
            _camera.transform.rotation.eulerAngles.x,
            _camera.transform.rotation.eulerAngles.y,
            _camera.transform.rotation.eulerAngles.z);
        
        _playerRayMarching.OnSetUp();

        gameStat.staticObjectTree = _staticObjectTree;
        
        if(gameStat.obbTest == null)return;
        // 一度だけコピー
        Mesh tempMesh = GameObject.Instantiate(gameStat.obbTest.GetComponent<MeshFilter>().mesh);
        // 頂点データ取得
        Vector3[] testMeshVertices = tempMesh.vertices;
        AlignedOBB testObb = new AlignedOBB(gameStat.obbTest, testMeshVertices);
        _obbVertices = testObb.Vertices;
        //OBB testObb = new OBB(test, testMeshVertices);
        // Debug.Log(testObb.Center);
        gameStat.obbTestObjects[0].position = testObb.Center;
        gameStat.obbTestObjects[1].position = testObb.Min;
        gameStat.obbTestObjects[2].position = testObb.Max;
    }

    public void OnUpdate()
    {
        Mesh tempMesh = GameObject.Instantiate(gameStat.obbTest.GetComponent<MeshFilter>().mesh);
        // 頂点データ取得
        Vector3[] testMeshVertices = tempMesh.vertices;
        AlignedOBB testObb = new AlignedOBB(gameStat.obbTest, testMeshVertices);
        
        Debug.Log($"ポイントの交差 : {testObb.IsSphereIntersection(gameStat.obbTestObjects[0].position, 1)}");
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
        //Debug.Log(string.Join($", ", _cameraCorners));
        
        // for (int i = 0; i < _cameraCorners.Count; i++)
        // {
        //     gameStat.testObjects[i].position = _cameraCorners[i];
        // }
        
        //_cameraBounds = JunGeometry.GetBoundsFromVertices(_cameraCorners);
        _cameraAABB3D = JunGeometry.GetAABB3DFromVertices(_cameraCorners);
        // Debug.Log($"{_cameraAABB3D.Max} , {_cameraAABB3D.Min}");
        // gameStat.testObjects[0].position = _cameraAABB3D.Max;
        // gameStat.testObjects[1].position = _cameraAABB3D.Min;
        //_cameraAABB3D = new AABB3D(_cameraBounds);
        
        // _playerRayMarching.SetBounds(_cameraBounds);
        // _playerRayMarching.SetAABB3D(_cameraAABB3D);
        //できた
        //_playerRayMarching.SetBound(cameraAABB3D);
        // Debug.Log(_staticObjectTree);
        //カメラが交差している可能性のあるオブジェクトたち staticに何もない場合は空のリスト
        //List<TreeNode3D> staticIntersectNodes = _staticObjectTree != null ? _staticObjectTree.GetIntersectNode(_cameraAABB3D) : new List<TreeNode3D>();
        List<TreeNode3D<(Transform, AlignedOBB)>> staticIntersectNodes = _staticObjectTree != null ? _staticObjectTree.GetIntersectNode(_cameraAABB3D) : new List<TreeNode3D<(Transform, AlignedOBB)>>();
        
        //カメラが交差しているモートン空間
        //int[] intersectMortonSpaceNums = JunGeometry.GetMortonCodesFromAABB(_cameraAABB3D, _mortonSpaceBasePos, _dimensionLevel, _cellSize);
        int[] intersectMortonSpaceNums = JunGeometry.GetMortonNumbersFromAABB(_cameraAABB3D, _mortonSpaceBasePos, _dimensionLevel, _cellSize);
        HashSet<int> cameraMortonNums = new HashSet<int>(intersectMortonSpaceNums);//含まれるかの処理のためHash化
        // Debug.Log(string.Join(", ", intersectMortonSpaceNums));
        
        //全ての動的オブジェクトに対して、カメラの交差しているモートン空間内にいるかどうかを確認、いたらリストに追加
        List<Transform> dynamicObjectsInCamera = new List<Transform>();
        for (int i = 0; i < _dynamicObjectList.Count; i++)
        {
            Transform dynamicTrans = _dynamicObjectList[i];
            int mortonNum = JunGeometry.PositionToMortonNumber(dynamicTrans.position, _mortonSpaceBasePos, _dimensionLevel, _cellSize);
            // Debug.Log(mortonNum);
            if (cameraMortonNums.Contains(mortonNum))
            {
                dynamicObjectsInCamera.Add(dynamicTrans);
            }
        }
        
        List<Transform> interactableObjectsInCamera = new List<Transform>();
        for (int i = 0; i < _InteractableObjectList.Count; i++)
        {
            Transform interactableTrans = _InteractableObjectList[i];
            int mortonNum = JunGeometry.PositionToMortonNumber(interactableTrans.position, _mortonSpaceBasePos, _dimensionLevel, _cellSize);
            // Debug.Log(mortonNum);
            if (cameraMortonNums.Contains(mortonNum))
            {
                interactableObjectsInCamera.Add(interactableTrans);
            }
        }
        
        (Transform transform, int objType, AlignedOBB obb)[] objectDataArray = 
            new (Transform transform, int objType, AlignedOBB obb)[staticIntersectNodes.Count + dynamicObjectsInCamera.Count + interactableObjectsInCamera.Count];

        //静的オブジェクト
        for (int i = 0; i < staticIntersectNodes.Count; i++)
        {
            TreeNode3D<(Transform, AlignedOBB)> node = staticIntersectNodes[i];
            objectDataArray[i] = (node.InformationTuple.Item1, 1, node.InformationTuple.Item2);
        }

        //動的オブジェクト
        for (int i = 0; i < dynamicObjectsInCamera.Count; i++)
        {
            objectDataArray[staticIntersectNodes.Count + i] = (dynamicObjectsInCamera[i], 2, null);
        }
        
        //触れるオブジェクト
        for (int i = 0; i < interactableObjectsInCamera.Count; i++)
        {
            objectDataArray[staticIntersectNodes.Count + dynamicObjectsInCamera.Count + i] = (interactableObjectsInCamera[i], 3, null);
        }
        
        //敵よりあとの番号は全部見え隠れして欲しいオブジェクト群
        int[] hitObjects = _playerRayMarching.OnRayMarchingUpdate(objectDataArray).Where(index => index > 0 && index >= staticIntersectNodes.Count).ToArray();
        
        //けす
        foreach (Transform obj in _oldMeshableList)
        {
            MeshChangable change = obj.GetComponent<MeshChangable>();
            change.EntityMeshDisable();
        }
        _oldMeshableList.Clear();
        
        List<string> hitDynamicObjectNames = new List<string>();
        //つける
        for (int i = 0; i < hitObjects.Length; i++)
        {
            int index = hitObjects[i];
            Transform obj = objectDataArray[index].transform;
            
            hitDynamicObjectNames.Add(obj.name);
            MeshChangable change = obj.GetComponent<MeshChangable>();
            
            if(change == null)continue;
            change.EntityMeshAble();
            _oldMeshableList.Add(obj);
        }
        // Debug.Log(string.Join(", ", hitDynamicObjectNames));

        #region OBB描画

        Gizmos.color = Color.red;
        int[,] edges = {
            {0, 1}, {1, 3}, {3, 2}, {2, 0}, // 底面
            {4, 5}, {5, 7}, {7, 6}, {6, 4}, // 上面
            {0, 4}, {1, 5}, {2, 6}, {3, 7}  // 側面
        };

        //Debug.LogWarning(_obbVertices.Length);
        // Debug.Log(edges.GetLength(0));
        if (_obbVertices.Length > 0)
        {
            for (int i = 0; i < edges.GetLength(0); i++)
            {
                Debug.DrawLine(_obbVertices[edges[i, 0]], _obbVertices[edges[i, 1]]);
            }
        }
        #endregion
        
        #region モートン空間描画
        float _cellWidth = gameStat.CellSize.x;
        float _cellHeight = gameStat.CellSize.y;
        float _cellDepth = gameStat.CellSize.z;
        Transform _baseTrans = gameStat.mortonSpaceBaseTrans;
        
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
                
                Debug.DrawLine(from, to);
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
                
                Debug.DrawLine(from, to);
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
                
                Debug.DrawLine(from, to);
            }
        }

        #endregion
    }
    
    public List<(AABB3D bounds, Transform transform, AlignedOBB allignedObb)> GetStaticObjectList(Transform staticObjectParent)
    {
        if (staticObjectParent.childCount < 1) return null;
        
        List<(AABB3D bounds, Transform transform, AlignedOBB)> objectList = new List<(AABB3D bounds, Transform transform, AlignedOBB)>();
        MeshRenderer[] meshesArray = JunExpandUnityClass.GetChildrenComponent<MeshRenderer>(staticObjectParent);
        MeshFilter[] meshFiltersArray = JunExpandUnityClass.GetChildrenComponent<MeshFilter>(staticObjectParent);
        Transform[] transformsArray = JunExpandUnityClass.GetChildrenComponent<Transform>(staticObjectParent);
        
        for (int i = 0; i < meshesArray.Length; i++)
        {
            objectList.Add((
                new AABB3D(meshesArray[i].bounds), 
                transformsArray[i],
                new AlignedOBB(transformsArray[i], meshFiltersArray[i].mesh.vertices)
            ));
        }
        
        return objectList;
    }
}
