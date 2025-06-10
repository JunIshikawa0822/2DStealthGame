using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// [CreateAssetMenu(menuName = "EQS/Strategy/Distance")]
[System.Serializable]
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
    [SerializeField]private bool _filterClose = false;
    [SerializeField]private bool _filterFar = false;
    
    public override void RunStrategy(int currentStrategyIndex, List<EnvQueryItem> items)
    {
        if(items == null)return;
        
        if(IsActive && distanceTo != null)
        {
            foreach(EnvQueryItem item in items)
            {
                float distance = Vector3.Distance(distanceTo.position, item.GetWorldPosition());
                
                // フィルタリング処理
                if (_mode == StrategyMode.Filtering || _mode == StrategyMode.Both)
                {
                    // 最小距離チェック
                    if (_filterClose && distance < _minDistance)
                    {
                        item.IsValid = false;
                    }

                    // 最大距離チェック
                    if (_filterFar && distance > _maxDistance)
                    {
                        item.IsValid = false;
                    }
                }

                // スコアリング処理
                if(_mode == StrategyMode.Scoring || _mode == StrategyMode.Both)
                {
                    Debug.Log("はい");
                    item.TestResults[currentStrategyIndex] = _invertScoring ? distance : 1f / (distance + 0.001f);
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

