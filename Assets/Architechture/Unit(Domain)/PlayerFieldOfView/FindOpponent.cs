using UnityEngine;
using System.Collections.Generic;
public class FindOpponent
{
    private LayerMask _targetMask;
    private LayerMask _obstacleMask;

    public FindOpponent(LayerMask targetLayer, LayerMask obstacleLayer)
    {
        _targetMask = targetLayer;
        _obstacleMask = obstacleLayer;
    }

    public List<Transform> FindVisibleTargets(float viewAngle, float viewRadius, Transform transform)
    {
        List<Transform> newVisibleTargets = new List<Transform>();

        //第一引数が中心座標、第二引数が球の半径、引数で指定した球が触れた敵を全て配列で返す
        //でかいほう
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, _targetMask);

        //でかいほう
        Calc(targetsInViewRadius, viewAngle);

        return newVisibleTargets;
        
        void Calc(Collider[] targetsInRadiusArray, float angle)
        {
            for (int i = 0; i < targetsInRadiusArray.Length; i++)
            {
                //敵のtransform
                Transform target = targetsInRadiusArray[i].transform;
                //MeshRenderer enemyMeshRenderer = target.GetComponent<MeshRenderer>();

                //敵の方向のベクトル（正規化）
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                //敵の方向がviewAngle内だったら
                if (Vector3.Angle(transform.forward, dirToTarget) < angle / 2)
                {
                    //敵のdistance
                    float dstToTarget = Vector3.Distance(transform.position, target.position);

                    //敵までのrayを飛ばして間に障害物がなければ
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, _obstacleMask))
                    {
                        //targetはvisible
                        //enemyMeshRenderer.enabled = true;
                        newVisibleTargets.Add(target);
                    }
                }
            }
        }
    }
}
