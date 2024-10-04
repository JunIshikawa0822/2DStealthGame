using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System;

public class DrawFieldOfView
{
    private LayerMask _targetMask;
    private LayerMask _obstacleMask;

    //見えているターゲットを保存するリスト
    //private List<Transform> _newVisibleTargets = new List<Transform>();
    //private List<Transform> _oldVisibleTargets = new List<Transform>();

    //解像度
    private float _meshResolution;
    private int _edgeResolveIterations;
    private float _edgeDstThreshold;

    private MeshFilter _viewMeshFilter;
    private Mesh _viewMesh;

    public DrawFieldOfView(
        MeshFilter meshFilter, Mesh mesh, 
        LayerMask targetLayer, LayerMask obstacleLayer, 
        float meshResolution, int edgeResolveIterations, float edgeDstThreshold)
    {
        _viewMeshFilter = meshFilter;
        _viewMesh = mesh;

        //名前をつけて識別しやすくする
        //viewMesh.name = "View Mesh";

        //sharedMeshは設定しなくてもよい　設定すると同じMeshを持つもの同士でMeshを共有し、メモリデータを削減できる
        _viewMeshFilter.sharedMesh = _viewMesh;

        _meshResolution = meshResolution;
        _edgeResolveIterations = edgeResolveIterations;
        _edgeDstThreshold = edgeDstThreshold;

        _targetMask = targetLayer;
        _obstacleMask = obstacleLayer;
    }

    // void Start()
    // {
    //     //Meshを用意
    //     _viewMesh = new Mesh();

    //     //名前をつけて識別しやすくする
    //     //viewMesh.name = "View Mesh";

    //     //sharedMeshは設定しなくてもよい　設定すると同じMeshを持つもの同士でMeshを共有し、メモリデータを削減できる
    //     _viewMeshFilter.sharedMesh = _viewMesh;
    //     //FindTargetWithDelay(0.2f).Forget();
    // }

    //メインスレッドで作業しないことで他の処理がスムーズ
    // private async UniTask FindTargetWithDelay(float delayTime)
    // {
    //     while(true)
    //     {
    //         _newVisibleTargets = FindVisibleTargets(viewAngle, viewRadius);

    //         //newVisibleTargetを描画
    //         DisplayVisibleTargets();

    //         //新旧を比較し、描画するリストを更新
    //         UnDisplayInvisibleTargets();
    //         _oldVisibleTargets = _newVisibleTargets;

    //         await UniTask.Delay((int)delayTime * 1000);
    //     }
    // }

    // public void FindAndDrawTargets(float viewAngle, float viewRadius, Transform transform)
    // {
    //     _newVisibleTargets = FindVisibleTargets(viewAngle, viewRadius, transform);

    //     //newVisibleTargetを描画
    //     DisplayVisibleTargets();

    //     //新旧を比較し、描画するリストを更新
    //     UnDisplayInvisibleTargets();

    //     _oldVisibleTargets = _newVisibleTargets;
    // }

    // void LateUpdate()
    // {
    //     DrawFieldOfView(viewAngle, viewRadius, _viewMesh);
    //     //DrawFieldOfView(roundViewAngle1, roundViewRadius1, viewRoundMesh);
    // }

    // private void DisplayVisibleTargets()
    // {
    //     foreach(Transform target in _newVisibleTargets)
    //     {
    //         AEntity entity = target.GetComponent<AEntity>();

    //         entity.OnEntityMeshAble();
    //     }
    // }

    // private void UnDisplayInvisibleTargets()
    // {
    //     foreach (Transform oldTarget in _oldVisibleTargets)
    //     {
    //         bool isInclude = false;

    //         foreach (Transform newTarget in _newVisibleTargets)
    //         {
    //             //newにoldが含まれていればok
    //             if (oldTarget == newTarget)
    //             {
    //                 isInclude = true;
    //                 break;
    //             }
    //         }

    //         //含まれていないならオフ
    //         if (isInclude == false)
    //         {
    //             AEnemy enemy = oldTarget.GetComponent<AEnemy>();

    //             enemy.OnEntityMeshDisable();
    //         }
    //     }
    // }

    // private List<Transform> FindVisibleTargets(float viewAngle, float viewRadius, Transform transform)
    // {
    //     List<Transform> newVisibleTargets = new List<Transform>();

    //     //第一引数が中心座標、第二引数が球の半径、引数で指定した球が触れた敵を全て配列で返す
    //     //でかいほう
    //     Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, _targetMask);

    //     //でかいほう
    //     Calc(targetsInViewRadius, viewAngle);

    //     return newVisibleTargets;
        
    //     void Calc(Collider[] targetsInRadiusArray, float angle)
    //     {
    //         for (int i = 0; i < targetsInRadiusArray.Length; i++)
    //         {
    //             //敵のtransform
    //             Transform target = targetsInRadiusArray[i].transform;
    //             //MeshRenderer enemyMeshRenderer = target.GetComponent<MeshRenderer>();

    //             //敵の方向のベクトル（正規化）
    //             Vector3 dirToTarget = (target.position - transform.position).normalized;

    //             //敵の方向がviewAngle内だったら
    //             if (Vector3.Angle(transform.forward, dirToTarget) < angle / 2)
    //             {
    //                 //敵のdistance
    //                 float dstToTarget = Vector3.Distance(transform.position, target.position);

    //                 //敵までのrayを飛ばして間に障害物がなければ
    //                 if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, _obstacleMask))
    //                 {
    //                     //targetはvisible
    //                     //enemyMeshRenderer.enabled = true;
    //                     newVisibleTargets.Add(target);
    //                 }
    //             }
    //         }
    //     }
    // }

    public void DrawFOV(float viewAngle, float viewRadius, Transform transform)
    {
        //stepCount = 角度に解像度を掛けたもの
        int stepCount = Mathf.RoundToInt(viewAngle * _meshResolution);

        //stepCountを角度で割る = 1°をどれぐらいの密度で描画するか
        float stepAngleSize = viewAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        //度数の分だけ行われる
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;

            //角度に対してRayを飛ばし、障害物を考慮した各頂点の値を格納する
            ViewCastInfo newViewCast = ViewCast(angle, viewRadius, transform);

            if (i > 0)
            {
                //隣の度数線における距離から現在の度数線における距離を引いた値が閾値より大きい＝隣の度数線における距離よりも現在の距離が短い＝衝突
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > _edgeDstThreshold;

                //隣の度数におけるhitと現在のhitが異なる＝衝突の差
                //両方ぶつかっているが、その距離に差がある＝衝突の差
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    //中間を取得、補完
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast, viewRadius, transform);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        //扇の先＋原点
        int vertexCount = viewPoints.Count + 1;
        //頂点メッシュ
        Vector3[] vertices = new Vector3[vertexCount];
        //triangles 配列はメッシュの三角形を定義します。三角形の数は (vertexCount - 2) で、各三角形は3つの頂点を持ちます。
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _viewMesh.Clear();

        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }


    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float radius, Transform transform)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeResolveIterations; i++)
        {
            //前と後の角度の中間の角度取得
            float angle = (minAngle + maxAngle) / 2;

            //その角度でRayを飛ばして情報を取得
            ViewCastInfo newViewCast = ViewCast(angle, radius, transform);

            //oldViewCastのAngleと中間角度におけるdstの差分を確認
            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > _edgeDstThreshold;

            //差がない
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
                //差がない場合は(newViewCast.point, Vector3.zero)
            }
            //差がある
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
                //差がある場合は(Vector3.zero, newViewCast.point)
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    //Rayを飛ばして、当たればその位置を、当たらなければ距離と半径に従って位置を返す
    private ViewCastInfo ViewCast(float globalAngle, float viewRadius, Transform transform)
    {
        Vector3 dir = DirFromAngle(globalAngle, true, transform);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, _obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal, Transform transform)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    private struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

}
