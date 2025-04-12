using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// --- プレイヤーの視線からカバーを判定するテスト ---
[CreateAssetMenu(menuName = "EQS/Strategy/Visibility")]
public class VisibilityStrategy : EnvQueryStrategy
{
    public enum StrategyMode
    {
        Scoring,    // 普通の評価モード
        Filtering,  // フィルタリングモード
        Both        // 両方を適用
    }
    
    [SerializeField] private StrategyMode _mode = StrategyMode.Scoring;
    [SerializeField] private LayerMask _coverMask;
    // trueなら見えていないものを除外、falseなら見えているものを除外
    [SerializeField]private bool _invertScoring = false;
    // 見えているときのスコア（値が大きいほど良い）
    [SerializeField]private float _visibleScore = 1.0f;
    // 見えていないときのスコア（値が小さいほど悪い）
    [SerializeField]private float _notVisibleScore = 0.0f;
    public Transform visionObject;
    
    public override void RunStrategy(int currentStrategyIndex, List<EnvQueryItem> items)
    {
        if(items == null)return;

        if (IsActive && visionObject != null)
        {
            foreach (EnvQueryItem item in items)
            {
                Vector3 worldPos = item.GetWorldPosition();
                Vector3 dir = worldPos - visionObject.position;
                float dist = dir.magnitude;
                bool isVisible = !Physics.Raycast(visionObject.position, dir.normalized, out RaycastHit hit, dist, _coverMask);

                if (_mode == StrategyMode.Filtering || _mode == StrategyMode.Both)
                {
                    item.IsValid = !(_invertScoring ^ isVisible);
                }

                if (_mode == StrategyMode.Scoring || _mode == StrategyMode.Both)
                {
                    item.TestResults[currentStrategyIndex] = item.TestResults[currentStrategyIndex] = isVisible ? _visibleScore : _notVisibleScore;
                }
            }
        }
        else
        {
            foreach(EnvQueryItem item in items)
            {
                item.TestResults[currentStrategyIndex] = 0.0f;
            }
        }
    }
    // public override void RunStrategy(int currentStrategyIndex, List<EnvQueryItem> items)
    // {
    //     foreach (EnvQueryItem item in items)
    //     {
    //         Vector3 worldPos = item.GetWorldPosition();
    //         Vector3 dir = worldPos - visionObject.position;
    //         float dist = dir.magnitude;
    //         bool blocked = Physics.Raycast(visionObject.position, dir.normalized, out RaycastHit hit, dist, coverMask);
    //         item.TestResults[currentStrategyIndex] = blocked ? 1f : 0f;
    //     }
    // }
}
