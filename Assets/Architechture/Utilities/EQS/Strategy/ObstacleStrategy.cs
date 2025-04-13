using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using JunUtilities;
[System.Serializable]
public class ObstacleStrategy : EnvQueryStrategy
{
    public AABB3DTree<(Transform, AlignedOBB)> _obstaclesTree;
    
    public override void RunStrategy(int currentStrategyIndex, List<EnvQueryItem> items)
    {
        if (items == null || items.Count == 0)return;
        if (_obstaclesTree == null) ;
            
        // すべての点の境界を計算するためのベクトル変数を初期化
        Vector3 min = Vector3.positiveInfinity;  // 最小値として「無限大」を設定
        Vector3 max = Vector3.negativeInfinity;  // 最大値として「マイナス無限大」を設定
        
        foreach (EnvQueryItem item in items)
        {
            if (item.IsValid)
            {
                // 有効なアイテムのみを対象に最小・最大座標を更新
                min = Vector3.Min(min, item.GetWorldPosition());  // 現在の最小値とアイテムの位置を比較し、各成分の小さい方を選択
                max = Vector3.Max(max, item.GetWorldPosition());  // 現在の最大値とアイテムの位置を比較し、各成分の大きい方を選択
            }
        }
        
        // 計算した境界に小さなマージン(0.001)を追加してでっかいAABBを作成
        AABB3D entireBoundsAABB = new AABB3D(min - Vector3.one * 0.001f, max + Vector3.one * 0.001f);
        // Vector3.oneは(1,1,1)を表し、それに0.001を掛けた(0.001,0.001,0.001)を引いたり足したりして少し余裕を持たせる
        
        // 全部の点を含むAABBと交差する可能性のあるOBBを検索
        List<TreeNode3D<(Transform, AlignedOBB)>> potentialIntersectNodes = _obstaclesTree.GetIntersectNode(entireBoundsAABB);
        // _staticObjectTreeはOBBを格納した二分木で、GetIntersectNodeメソッドで交差するノードを取得
        
        // 各クエリアイテムを順番に処理
        foreach (EnvQueryItem item in items)
        {
            if (!item.IsValid) continue;  // すでに無効なアイテムはスキップ
                
            // アイテムの位置を取得
            Vector3 point = item.GetWorldPosition();
            
            // 準備したすべてのOBBと衝突判定
            foreach (TreeNode3D<(Transform, AlignedOBB)> node in potentialIntersectNodes)
            {
                bool isIntersect = node.InformationTuple.Item2.IsPointIntersection(point);

                if (isIntersect)
                {
                    item.IsValid = false;
                    break;
                }
            }
        }
    }
}
