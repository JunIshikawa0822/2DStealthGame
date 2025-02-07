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
    private void Start()
    {
        _playerMeshRenderer = player.GetComponent<MeshRenderer>();
        
        List<(AABB3D bounds, Transform transform)> objectList = new List<(AABB3D bounds, Transform transform)>();
        MeshRenderer[] boundsArray = JunExpandUnityClass.GetChildrenComponent<MeshRenderer>(staticObjectParent);
        Transform[] transformsArray = JunExpandUnityClass.GetChildrenComponent<Transform>(staticObjectParent);
        
        Debug.Log($"Boundsのリスト　{string.Join((","), boundsArray.Select(item => item.transform.name))}");
        Debug.Log($"Transformのリスト {string.Join((","), transformsArray.Select(item => item.transform.name))}");

        for (int i = 0; i < boundsArray.Length; i++)
        {
            objectList.Add((new AABB3D(boundsArray[i].bounds), transformsArray[i]));
        }

        _stageObjectTree = new AABB3DTree();
        _stageObjectTree.BuildTree(objectList);
    }

    private void Update()
    {
        //プレイヤーのAABB
        AABB3D playerAABB3D = new AABB3D(_playerMeshRenderer.bounds);
        //AABBが交差しているオブジェクトを探そう
        //AABBが交差している = オブジェクトとぶつかっている　というわけではない
        //オブジェクトが回転している可能性があるので、さらにここから判定を入れる
        List<(AABB3D bounds, Transform transform)> intersects = _stageObjectTree.GetIntersectAABB3D(playerAABB3D);
        Debug.Log(string.Join((","), intersects.Select(item => item.transform.name)));
        
        
    }
}
