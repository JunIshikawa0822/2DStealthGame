using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "EQS/Strategy/Distance")]
public class DistanceStrategy : EnvQueryStrategy
{
    public enum StrategyMode
    {
        Scoring,    // 普通の評価モード
        Filtering,  // フィルタリングモード
        Both        // 両方を適用
    }
    [SerializeField] private StrategyMode _mode = StrategyMode.Scoring;
    //これより近い点を除く
    [SerializeField]private float _minDistance = 10.0f;
    //これより遠い点を除く
    [SerializeField]private float _maxDistance = 100.0f;
    // trueなら近いほど高スコア、falseなら遠いほど高スコア
    [SerializeField]private bool _invertScoring = false;
    public Transform distanceTo;
    private bool _filterTooClose = true;
    private bool _filterTooFar = false;
    
    public override void RunStrategy(int currentStrategyIndex, List<EnvQueryItem> items)
    {
        if(items == null)return;
        
        if(IsActive && distanceTo != null)
        {
            foreach(EnvQueryItem item in items)
            {
                float distance = Vector3.Distance(distanceTo.position, item.GetWorldPosition());
                
                // フィルタリング処理
                if(_mode == StrategyMode.Filtering || _mode == StrategyMode.Both)
                {
                    // 最小距離チェック
                    if(_filterTooClose && distance < _minDistance)
                    {
                        item.IsValid = false;
                    }
                    
                    // 最大距離チェック
                    if(_filterTooFar && distance > _maxDistance)
                    {
                        item.IsValid = false;
                    }
                }
                
                // スコアリング処理
                if(_mode == StrategyMode.Scoring || _mode == StrategyMode.Both)
                {
                    item.TestResults[currentStrategyIndex] = distance;
                }
                else
                {
                    // フィルタリングのみの場合はスコアに影響させない
                    item.TestResults[currentStrategyIndex] = 0.0f;
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
}

