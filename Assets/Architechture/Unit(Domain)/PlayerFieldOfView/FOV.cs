using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using Unity.Entities.UniversalDelegates;

public class FOV : MonoBehaviour
{
    [SerializeField] private float _viewRadius;
    [Range(0, 360), SerializeField]private float _viewAngle;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    //見えているターゲットを保存するリスト
    private HashSet<Transform> _previousVisibleTargets = new HashSet<Transform>();
    private HashSet<Transform> _currentVisibleTargets = new HashSet<Transform>();

    //解像度
    [SerializeField] private float meshResolution;
    [SerializeField] private int edgeResolveIterations;
    [SerializeField] private float edgeDstThreshold;
    [SerializeField] private bool isTargetDisplayChange;
    [SerializeField] private bool isDrawFieldOfView;

    [SerializeField] private MeshFilter viewMeshFilter;
    private Mesh _viewMesh;
    private CancellationTokenSource _cts;

    public float ViewRadius {get => _viewRadius; set => _viewRadius = value;}
    public float ViewAngle {get => _viewAngle; set => _viewAngle = value;}
    
    private Collider[] _targetsBuffer = new Collider[100]; // 事前に配列を確保
    private List<Vector3> _viewPoints = new List<Vector3>();
    
    public IReadOnlyCollection<Transform> CurrentVisibleTargets => _currentVisibleTargets;

    /// <summary>
    /// 現在見えているターゲットのTransformをリストで取得
    /// 内部でリアルタイム更新を実行
    /// </summary>
    public List<Transform> GetVisibleTargetsList()
    {
        FindCurrentVisibleTargets();
        return _currentVisibleTargets.ToList();
    }

    /// <summary>
    /// 現在見えているターゲットのTransformを配列で取得
    /// 内部でリアルタイム更新を実行
    /// </summary>
    public Transform[] GetVisibleTargetsArray()
    {
        FindCurrentVisibleTargets();
        Transform[] targets = new Transform[_currentVisibleTargets.Count];
        _currentVisibleTargets.CopyTo(targets);
        return targets;
    }

    void Start()
    {
        //Meshを用意
        InitializeViewMesh();
        _cts = new CancellationTokenSource();
        FindAndDrawTargetWithDelay(0.2f, _cts.Token).Forget();
    }
    
    private void InitializeViewMesh()
    {
        _viewMesh = new Mesh();
        _viewMesh.name = "View Mesh";
        
        if (viewMeshFilter != null)
        {
            viewMeshFilter.sharedMesh = _viewMesh;
        }
    }

    private async UniTask FindAndDrawTargetWithDelay(float delayTime, CancellationToken token)
    {
        
        int delayMs = (int)(delayTime * 1000);
        
        while (!token.IsCancellationRequested)
        {
            UpdateVisibleTargets();
            await UniTask.Delay(delayMs, cancellationToken: token);
        }
    }
    
    private void UpdateVisibleTargets()
    {
        // 現在の可視ターゲットを取得
        FindCurrentVisibleTargets();
        
        if (!isTargetDisplayChange) return;
        
        // Dirty Flagパターンを使用した効率的な状態管理
        ProcessVisibilityChanges();
        
        // 次のフレームのために前回の状態を保存
        SwapTargetSets();
    }
    
    private void FindCurrentVisibleTargets()
    {
        _currentVisibleTargets.Clear();
        
        // OverlapSphereNonAllocを使用してガベージコレクションを削減
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _viewRadius, _targetsBuffer, targetMask);
        
        for (int i = 0; i < hitCount; i++)
        {
            Transform target = _targetsBuffer[i].transform;
            
            if (IsTargetVisible(target))
            {
                _currentVisibleTargets.Add(target);
            }
        }
    }
    
    private bool IsTargetVisible(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        
        // 視野角チェック
        if (Vector3.Angle(transform.forward, dirToTarget) >= _viewAngle / 2) 
            return false;
        
        // 障害物チェック
        float dstToTarget = Vector3.Distance(transform.position, target.position);
        return !Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask);
    }
    
    //今のフレームで見えるものとそうでないものを変更
    private void ProcessVisibilityChanges()
    {
        // 新しく見えるようになったターゲット（OFF → ON）
        foreach (Transform target in _currentVisibleTargets)
        {
            if (!_previousVisibleTargets.Contains(target))
            {
                SetTargetVisibility(target, true);
            }
        }
        
        // 見えなくなったターゲット（ON → OFF）
        foreach (Transform target in _previousVisibleTargets)
        {
            if (!_currentVisibleTargets.Contains(target))
            {
                SetTargetVisibility(target, false);
            }
        }
        
        // 継続して見えているターゲット（ON → ON）と
        // 継続して見えていないターゲット（OFF → OFF）は何もしない
    }
    
    private void SetTargetVisibility(Transform target, bool visible)
    {
        MeshChangeable meshChanger = target.GetComponent<MeshChangeable>();
        if (meshChanger != null)
        {
            meshChanger.SetVisibility(visible);
        }
    }
    
    private void SwapTargetSets()
    {
        // 参照を入れ替える（GCを避けるため）
        HashSet<Transform> temp = _previousVisibleTargets;
        _previousVisibleTargets = _currentVisibleTargets;
        _currentVisibleTargets = temp;
    }
    
    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    void LateUpdate()
    {
        if(_viewMesh == null || isDrawFieldOfView)return;
        DrawFieldOfView(_viewAngle, _viewRadius, _viewMesh);
        //DrawFieldOfView(roundViewAngle1, roundViewRadius1, viewRoundMesh);
    }

    void DrawFieldOfView(float viewAngle, float viewRadius, Mesh mesh)
    {
        //stepCount = 角度に解像度を掛けたもの
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);

        //stepCountを角度で割る = 1°をどれぐらいの密度で描画するか
        float stepAngleSize = viewAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        //Debug.Log("OldViewCast = hit : " + oldViewCast.hit + ", point : " + oldViewCast.point + ", dst : " + oldViewCast.dst + ", angle : " + oldViewCast.angle);
        //Debug.Log(oldViewCast.dst);
        //度数の分だけ行われる
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;

            //角度に対してRayを飛ばし、障害物を考慮した各頂点の値を格納する
            ViewCastInfo newViewCast = ViewCast(angle, viewRadius);
            //Debug.Log("NewViewCast = hit : " + newViewCast.hit + ", point : " + newViewCast.point + ", dst : " + newViewCast.dst + ", angle : " + newViewCast.angle);

            if (i > 0)
            {
                //隣の度数線における距離から現在の度数線における距離を引いた値が閾値より大きい＝隣の度数線における距離よりも現在の距離が短い＝衝突
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                //隣の度数におけるhitと現在のhitが異なる＝衝突の差
                //両方ぶつかっているが、その距離に差がある＝衝突の差
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    //中間を取得、補完
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast, viewRadius);
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

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }


    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float radius)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            //前と後の角度の中間の角度取得
            float angle = (minAngle + maxAngle) / 2;

            //その角度でRayを飛ばして情報を取得
            ViewCastInfo newViewCast = ViewCast(angle, radius);

            //oldViewCastのAngleと中間角度におけるdstの差分を確認
            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;

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
    ViewCastInfo ViewCast(float globalAngle, float viewRadius)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
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

    public struct EdgeInfo
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
