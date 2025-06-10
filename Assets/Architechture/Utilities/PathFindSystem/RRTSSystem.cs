using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class RRTSSystem : MonoBehaviour
{
    private List<RRTSNode> _rrtsNodes = new List<RRTSNode>();
    //一定方向にどれだけ進むんだい
    [SerializeField]private float _stepDistance;
    //効率的なノードにするために、近くのノードができた場合に対処するのが通常のRRTとの違い（*）
    //近くにあるノードを調べるために一定の範囲を設定しておく 
    //_stepLengthよりはちょい大きめがいいかも？
    [SerializeField]private float _neighborRadius;
    //どこまで近づいたらごーるとするか
    [SerializeField]private float _thresholdDistance;
    // 最大試行回数
    [SerializeField]private int _maxIterations = 5000;
    // ゴール方向を基準として設定する確率
    [SerializeField]private float _goalBias = 0.1f;
    // サンプリング範囲
    public float minRange = -100;
    public float maxRange = 100;

    //ステージの中心座標
    [SerializeField] private Transform _stageCenterTrans;
    private Vector3 _stageCenter = default;

    //線分との当たり判定　中身は外付けでいけるのでいいね
    private Func<Vector3, Vector3, bool> _collideFunc;

    public List<Vector3> FindPath(Vector3 startPos, Vector3 goalPos, Func<Vector3, Vector3, bool> collideFunc)
    {
        if (_stageCenterTrans != null) _stageCenter = _stageCenterTrans.position;
        
        _collideFunc = collideFunc;
        _rrtsNodes.Clear(); // 初期化
        _rrtsNodes.Add(new RRTSNode(startPos, null, 0f));

        RRTSNode goalNode = null;
        float bestGoalDistance = float.MaxValue;

        for(int i = 0; i < _maxIterations; i++)
        {
            //適当な点を取ってくる
            Vector3 randomPoint = GetRandomPoint(goalPos);

            //これまで辿ってきたNodeのなかから適当な点と一番近い点のノードを選ぶ
            RRTSNode nearestNode = GetNearestNode(randomPoint);

            //ノードの点から適当な点の方向にある程度(stepDistanceぶん)進める
            Vector3 newPoint = VectorStep(nearestNode.Position, randomPoint);
            
            // 障害物チェック
            if (!IsCollide(nearestNode.Position, newPoint))
            {
                // コスト計算
                float newCost = nearestNode.Cost + Vector3.Distance(nearestNode.Position, newPoint);
                
                // Debug.Log($"RandomPoint : {randomPoint}");
                // Debug.Log($"NewPoint : {newPoint}");
                //
                RRTSNode newNode = new RRTSNode(newPoint, nearestNode, newCost);
                _rrtsNodes.Add(newNode);

                // 近傍ノードを探して最適化 ここがRRT*アルゴリズムの肝だよ
                OptimizeWithNeighbors(newNode);

                // ゴールとの距離をチェック
                float distToGoal = Vector3.Distance(newPoint, goalPos);
                if (distToGoal < _thresholdDistance)
                {
                    // ゴールに到達したノードを記録
                    if (distToGoal < bestGoalDistance)
                    {
                        bestGoalDistance = distToGoal;
                        goalNode = newNode;
                    }
                }

                // デバッグ情報
                if (i % 100 == 0)
                {
                    // Debug.Log($"反復: {i}, 最良のゴール距離: {bestGoalDistance}");
                }
            }
        }

        //ゴールが見つかればそこまでの経路を返すよ
        if (goalNode != null)
        {
            Debug.Log("経路を発見しました！");
            return ConstructPath(goalNode);
        }

        //ゴールが見つからないなら一番ゴールまで近い場所を返す
        if (_rrtsNodes.Count > 1)
        {
            RRTSNode closestToGoal = null;
            float minDist = float.MaxValue;
            
            foreach (RRTSNode node in _rrtsNodes)
            {
                float dist = Vector3.Distance(node.Position, goalPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestToGoal = node;
                }
            }
            
            Debug.Log($"完全な経路は見つかりませんでした。最も近いノードまでの距離: {minDist}");
            return ConstructPath(closestToGoal);
        }

        //無駄足
        return null;
    }
    
    private bool IsCollide(Vector3 start, Vector3 end)
    {
        Debug.Log($"{start}, {end}, {_collideFunc}");
        return _collideFunc(start, end);
    }

    private void OptimizeWithNeighbors(RRTSNode newNode)
    {
        // 近傍ノードを取得
        List<RRTSNode> neighbors = GetNeighborNodes(newNode);
        
        // 近傍ノードへの再ワイヤリング（RRT* の核心部分）
        foreach (RRTSNode neighbor in neighbors)
        {
            // 自分自身と親は除外
            if (neighbor == newNode || neighbor == newNode.Parent)
                continue;
                
            // この近傍ノードへの直接パスに障害物がないか確認
            if (!IsCollide(newNode.Position, neighbor.Position))
            {
                // 新しいコストを計算
                float potentialCost = neighbor.Cost + Vector3.Distance(neighbor.Position, newNode.Position);
                
                // より良いパスが見つかれば更新
                if (potentialCost < newNode.Cost)
                {
                    newNode.Parent = neighbor;
                    newNode.Cost = potentialCost;
                }
            }
        }
        
        // 近傍ノードのコスト最適化（これもRRT* の重要部分）
        foreach (RRTSNode neighbor in neighbors)
        {
            // 自分自身は除外
            if (neighbor == newNode)
                continue;
                
            // 新しいノードを経由したパスに障害物がないか確認
            if (!IsCollide(newNode.Position, neighbor.Position))
            {
                // 新しいコストを計算
                float potentialCost = newNode.Cost + Vector3.Distance(newNode.Position, neighbor.Position);
                
                // より良いパスが見つかれば更新
                if (potentialCost < neighbor.Cost)
                {
                    neighbor.Parent = newNode;
                    neighbor.Cost = potentialCost;
                }
            }
        }
    }
    
    private RRTSNode GetNearestNode(Vector3 point)
    {
        RRTSNode nearestNode = null;
        float minDistance = float.MaxValue;

        foreach (RRTSNode node in _rrtsNodes)
        {
            //Nodeに入っている各点データとの距離
            float distance = Vector3.Distance(node.Position, point);
            
            //近いものをえらぶ
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestNode = node; // 最も近いノードを更新
            }
        }

        return nearestNode; // 最も近いノードを返す
    }

    private Vector3 GetRandomPoint(Vector3 goal)
    {
        // ゴールにバイアスをかける
        if (UnityEngine.Random.value < _goalBias)
        {
            return goal;
        }
        else
        {
            float randomX = UnityEngine.Random.Range(minRange, maxRange);
            float randomY = UnityEngine.Random.Range(minRange, maxRange);
            float randomZ = UnityEngine.Random.Range(minRange, maxRange);
            return new Vector3(randomX + _stageCenter.x, _stageCenter.y, randomZ + _stageCenter.z);
        }
    }
    
    //適当な点に向かって一定の長さだけ歩みを進める処理
    private Vector3 VectorStep(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized; // 方向を計算
        return from + direction * _stepDistance; // 新しいポイントを計算
    }

    private List<RRTSNode> GetNeighborNodes(RRTSNode node)
    {
        List<RRTSNode> neighbors = new List<RRTSNode>();

        foreach (RRTSNode rrtsNode in _rrtsNodes)
        {
            //近いか判定
            if (Vector3.Distance(rrtsNode.Position, node.Position) < _neighborRadius)
            {
                neighbors.Add(rrtsNode); // 近傍ノードとして追加
            }
        }

        return neighbors; // 近傍ノードのリストを返す
    }
    
    private List<Vector3> ConstructPath(RRTSNode endNode)
    {
        List<Vector3> path = new List<Vector3>();
        RRTSNode currentNode = endNode;

        //さかのぼっていく（最初のNodeはかならず親がnullのはず）
        while (currentNode != null)
        {
            path.Add(currentNode.Position); // 経路を構築
            currentNode = currentNode.Parent; // 親ノードを辿る
        }
        
        path.Reverse(); // 逆順にして開始点からの経路にする
        return path; // 経路を返す
    }
}

