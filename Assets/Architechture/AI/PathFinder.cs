using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JunUtilities;
using UnityEditor.ShaderGraph.Internal;

public class PathFinder : MonoBehaviour
{
    private AABB3DTree<(Transform, AlignedOBB)> _staticObjectTree;
    [SerializeField]private Vector3 _enemySize;
    [SerializeField] private Transform goal;
    [SerializeField] private GameObject pathTestObject;

    [SerializeField] private Transform[] LineTestObjects;
    [SerializeField] private Transform stageCenter;
    public void SetUp(AABB3DTree<(Transform, AlignedOBB)> tree)
    {
        _staticObjectTree = tree;

        RRTStar moveAlgorithm = new RRTStar
        (
            5f,
            15,
            2,
            IsLineCollideWithStaticObject,
            200,
            0.2f,
            -100,
            100,
            stageCenter.transform.position
        );
        
        //Debug.Log(IsLineCollideWithStaticObject(LineTestObjects[0].position, LineTestObjects[1].position));

        List<Vector3> paths = moveAlgorithm.FindPath(this.transform.position, goal.position);
        if (paths == null)
        {
            Debug.Log("path null");
        }
        else if (paths.Count == 0)
        {
            Debug.Log("path empty");
        }
        else
        {
            Debug.Log(string.Join(", ", paths));
            foreach (Vector3 pos in paths)
            {
                Debug.Log($"位置 : {pos}");
                Vector3 groundPos = new Vector3(pos.x, stageCenter.position.y, pos.z);
                Instantiate(pathTestObject, groundPos, Quaternion.identity);
            }
        }
    }

    private void Update()
    {
        //Debug.Log(IsLineCollideWithStaticObject(LineTestObjects[0].position, LineTestObjects[1].position));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        if(!Application.isPlaying)return;
        Gizmos.DrawLine(LineTestObjects[0].position, LineTestObjects[1].position);
    }
    
    public bool IsLineCollideWithStaticObject(Vector3 startPos, Vector3 endPos)
    {
        AABB3D lineBound = new AABB3D(startPos, endPos);
        List<TreeNode3D<(Transform, AlignedOBB)>> intersectNodes = _staticObjectTree.GetIntersectNode(lineBound);

        foreach (TreeNode3D<(Transform transform, AlignedOBB allignedObb)> node in intersectNodes)
        {
            // Debug.Log($"オブジェクト : {node.InformationTuple.transform.name}, " +
            //           $"サイズ : {node.InformationTuple.allignedObb.Size}");
            bool isCollide = node.InformationTuple.allignedObb.IsLineIntersectionSAT(startPos, endPos);

            if (isCollide)
            {
                return true;
            }
        }
        return false;
    }
}
