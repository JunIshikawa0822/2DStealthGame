using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JunUtilities;
using System.Linq;

public class StageObject : MonoBehaviour
{
    public Transform staticObjectParent;
    public Transform player;
    private MeshRenderer _playerMeshRenderer;

    private AABB3DTree _stageObjectTree;

    public Transform obbTest;
    private Vector3[] _obbTestPoints = new Vector3[8];
    private Vector3[] vecs = new Vector3[3];
    private void Start()
    {
        _playerMeshRenderer = player.GetComponent<MeshRenderer>();
        
        List<(AABB3D bounds, OBB orientedBounds, Transform transform)> objectList = new List<(AABB3D bounds, OBB orientedBounds, Transform transform)>();
        MeshRenderer[] meshesArray = JunExpandUnityClass.GetChildrenComponent<MeshRenderer>(staticObjectParent);
        MeshFilter[] meshFiltersArray = JunExpandUnityClass.GetChildrenComponent<MeshFilter>(staticObjectParent);
        Transform[] transformsArray = JunExpandUnityClass.GetChildrenComponent<Transform>(staticObjectParent);
        
        // Debug.Log($"Boundsのリスト　{string.Join((","), boundsArray.Select(item => item.transform.name))}");
        //Debug.Log($"Transformのリスト {string.Join((","), transformsArray.Select(item => item.transform.name))}");

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
    }

    private void Update()
    {
        //プレイヤーのAABB
        AABB3D playerAABB3D = new AABB3D(_playerMeshRenderer.bounds);
        //AABBが交差しているオブジェクトを探そう
        //AABBが交差している = オブジェクトとぶつかっている　というわけではない
        //オブジェクトが回転している可能性があるので、さらにここから判定を入れる
        // List<(AABB3D bounds, OBB orientedBounds, Transform transform)> intersectObjects = _stageObjectTree.GetIntersectAABB3D(playerAABB3D);
        // Debug.Log(string.Join((","), intersectObjects.Select(item => item.transform.name)));
        List<TreeNode3D> intersectNodes = _stageObjectTree.GetIntersectNode(playerAABB3D);
        // Debug.Log(intersectNodes.Count);
        Debug.Log(
            string.Join((","), 
                intersectNodes
                    .Where(item => item != null && item.Transform != null)
                    .Select(item => item.Transform)));
        Debug.Log(
            string.Join((","), 
                intersectNodes
                    .Where(item => item != null && item.OrientedBounds != null)
                    .Select(item => item.OrientedBounds.Center)));
        
    }

    public void OnDrawGizmos()
    {
        if(!Application.isPlaying)return;
        
        Gizmos.color = Color.blue;
        // foreach (Vector3 point in vecs)
        // {
        //     Gizmos.DrawLine(point, point * 20);
        // }
        // Gizmos.color = Color.green;
        int[,] edges = {
            {0, 1}, {1, 3}, {3, 2}, {2, 0}, // 底面
            {4, 5}, {5, 7}, {7, 6}, {6, 4}, // 上面
            {0, 4}, {1, 5}, {2, 6}, {3, 7}  // 側面
        };
        
        // foreach (Vector3 point in _obbTestPoints)
        // {
        //     Debug.Log(point);
        // }
        
        for (int i = 0; i < edges.GetLength(0); i++)
        {
            Gizmos.DrawLine(_obbTestPoints[edges[i, 0]], _obbTestPoints[edges[i, 1]]);
        }
    }
}
