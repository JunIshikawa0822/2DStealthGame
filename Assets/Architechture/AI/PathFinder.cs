using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JunUtilities;
using UnityEditor.ShaderGraph.Internal;
using Cysharp.Threading.Tasks;
using System.Threading;

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
            4f,
            15,
            2,
            IsLineCollideWithStaticObject,
            1000,
            0.2f,
            -100,
            100,
            stageCenter.transform.position
        );

        RRTS moveAlgorithm2 = new RRTS
        (
            stageCenter.position,
            4,
            15,
            2,
            IsLineCollideWithStaticObject,
            5000,
            0.1f,
            -100,
            100
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
                //Debug.Log($"位置 : {pos}");
                Vector3 groundPos = new Vector3(pos.x, stageCenter.position.y, pos.z);
                Instantiate(pathTestObject, groundPos, Quaternion.identity);
            }
        }
        
        MoveAlongPaths(paths);
    }

    private void Update()
    {
        //Debug.Log(IsLineCollideWithStaticObject(LineTestObjects[0].position, LineTestObjects[1].position);
    }

    private void FixedUpdate()
    {
        //Debug.Log("はあ");
        //MoveToDir(goal.position - transform.position);
    }

    // private void MoveToDir(Vector3 moveDir)
    // {
    //     while ((goal.position - transform.position).sqrMagnitude > 1)
    //     {
    //         Vector3 direction = moveDir.normalized;
    //         transform.localPosition += direction * 5;
    //     }
    // }
    //
    // private async UniTask Move(List<Vector3> paths)
    // {
    //     await MoveAlongPaths(paths);
    // }

    private async UniTask MoveAlongPaths(List<Vector3> paths)
    {
        foreach (Vector3 target in paths)
        {
            await MoveToDir(target);
        }
        
        Debug.Log("おわった");
    }

    private async UniTask MoveToDir(Vector3 target)
    {
        float stuckTimeThreshold = 2.0f;
        float noMovementTimer = 0f;
        Vector3 lastPosition = transform.localPosition;
        float movementThreshold = 0.1f;
        // 目標に到達するまでループ
        while ((target - transform.position).sqrMagnitude > _enemySize.y)
        {
            Debug.Log("移動中");
            Debug.Log($"ターゲット : {target}, 現在 : {transform.position}, 距離 : {(target - transform.position).sqrMagnitude}");
            
            // 目標方向の単位ベクトルを算出し、localPositionに加算して移動
            Vector3 direction = (target - transform.position).normalized;
            transform.localPosition += direction * 5 * Time.deltaTime;

            if ((transform.localPosition - lastPosition).sqrMagnitude < movementThreshold)
            {
                noMovementTimer += Time.deltaTime;
                if (noMovementTimer >= stuckTimeThreshold)
                {
                    Debug.Log("一定時間動いていないため移動を中断します。");
                    return;
                }
            }
            else
            {
                noMovementTimer = 0f;
                lastPosition = transform.localPosition;
            }

            // 次のフレームの Update 時に処理を再開
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        
        Debug.Log("1フェーズ終了");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        if(!Application.isPlaying)return;
        Gizmos.DrawLine(LineTestObjects[0].position, LineTestObjects[1].position);
    }

    public bool IsOBBCollisionWithStaticObject(Vector3 startPos, Vector3 endPos)
    {
        return true;
    }
    
    public bool IsLineCollideWithStaticObject(Vector3 startPos, Vector3 endPos)
    {
        AABB3D lineBound = new AABB3D(startPos, endPos);
        List<TreeNode3D<(Transform, AlignedOBB)>> intersectNodes = _staticObjectTree.GetIntersectNode(lineBound);

        foreach (TreeNode3D<(Transform transform, AlignedOBB allignedObb)> node in intersectNodes)
        {
            // Debug.Log($"オブジェクト : {node.InformationTuple.transform.name}, " +
            //           $"サイズ : {node.InformationTuple.allignedObb.Size}");
            bool isCollide = node.InformationTuple.allignedObb.IsThickLineIntersection(startPos, endPos, _enemySize.x, _enemySize.y);

            if (isCollide)
            {
                return true;
            }
        }
        return false;
    }

    public void OnDestroy()
    {
        
    }
}
