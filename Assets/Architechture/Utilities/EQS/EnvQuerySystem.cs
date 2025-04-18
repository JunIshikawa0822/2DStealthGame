
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using JunUtilities;

public class EnvQuerySystem : MonoBehaviour
{
    [SerializeField] private bool isActiveSystem = true;
    public GameObject centerOfItems;
    [SerializeField]private float _itemRadius = 4.0f;
    [SerializeField]private float _itemSpaceBetween = 1.0f;
    [SerializeField]private EnvItemGeneratorType _generatorType = EnvItemGeneratorType.Circle;
    
    [SerializeReference]private List<EnvQueryStrategy> _envQueryStrategies = new List<EnvQueryStrategy>();
    private List<EnvQueryItem> _envQueryItems;
    private GameObject _querier;
    private IGeneratorBase _generator;
    
    public EnvQueryItem BestResult { get; private set; }
    public List<EnvQueryItem> BestResults { get; private set; } = new List<EnvQueryItem>();

    public enum EnvItemGeneratorType
    {
        Circle,
        Grid
    }
    
    public void OnSetUp(AABB3DTree<(Transform, AlignedOBB)> obstacles)
    {
        BestResults = new List<EnvQueryItem>();
        
        if(_querier == null)
        {
            _querier = this.gameObject;
        }
        
        if(centerOfItems == null)
        {
            centerOfItems = _querier;
        }
        
        if(_generatorType == EnvItemGeneratorType.Grid) _generator = new GenerateGrid(_itemRadius, _itemSpaceBetween);
        
        if(centerOfItems != null && _generator != null)
        {
            _envQueryItems = _generator.GenerateItems(_envQueryStrategies.Count, centerOfItems.transform);
        }
        else
        {
            _envQueryItems = new List<EnvQueryItem>();
        }

        if(_envQueryStrategies.Count != 0 || obstacles != null)
        {
            foreach (EnvQueryStrategy strategy in _envQueryStrategies)
            {
                if (strategy is ObstacleStrategy obstacleStrategy) obstacleStrategy.obstaclesTree = obstacles;
            }
        }
    }

    public void Update()
    {
        if(isActiveSystem == false)return;
        
        ResetScore();
        for(int currentStrategy = 0; currentStrategy < _envQueryStrategies.Count; currentStrategy++)
        {
            _envQueryStrategies[currentStrategy].RunStrategy(currentStrategy, _envQueryItems);
            _envQueryStrategies[currentStrategy].NormalizeItemScores(currentStrategy, _envQueryItems);
        }
        
        NormalizeScore();
        
        float maxScore = _envQueryItems
            .Where(x => x.IsValid)
            .Max(x => x.Score);
        
        BestResults = _envQueryItems
            .Where(x => x.IsValid && Mathf.Approximately(x.Score, maxScore))
            .ToList();
        
        // BestResult = _envQueryItems
        //     .Where(x => x.IsValid)
        //     .OrderByDescending(x => x.Score)
        //     .FirstOrDefault();
    }

    public List<Vector3> FindPoints()
    {
        ResetScore();
        for(int currentStrategy = 0; currentStrategy < _envQueryStrategies.Count; currentStrategy++)
        {
            _envQueryStrategies[currentStrategy].RunStrategy(currentStrategy, _envQueryItems);
            _envQueryStrategies[currentStrategy].NormalizeItemScores(currentStrategy, _envQueryItems);
        }
        
        NormalizeScore();
        
        float maxScore = _envQueryItems
            .Where(x => x.IsValid)
            .Max(x => x.Score);

        BestResults = _envQueryItems
            .Where(x => x.IsValid && Mathf.Approximately(x.Score, maxScore))
            .ToList();

        return BestResults
            .Select(item => item.GetWorldPosition())
            .ToList();
    }

    public Vector3 FindPoint()
    {
        ResetScore();
        for(int currentStrategy = 0; currentStrategy < _envQueryStrategies.Count; currentStrategy++)
        {
            _envQueryStrategies[currentStrategy].RunStrategy(currentStrategy, _envQueryItems);
            _envQueryStrategies[currentStrategy].NormalizeItemScores(currentStrategy, _envQueryItems);
        }
        
        NormalizeScore();
        BestResult = _envQueryItems
            .Where(x => x.IsValid)
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        return BestResult.GetWorldPosition();
    }
    
    private void NormalizeScore()
    {
        if(_envQueryItems == null || _envQueryItems.Count < 1)
        {
            return;
        }

        float maxScore = _envQueryItems[0].Score;
        float minScore = _envQueryItems[0].Score;

        foreach(EnvQueryItem item in _envQueryItems)
        {
            if(item.Score > maxScore)
            {
                maxScore = item.Score;
            }
            if(item.Score < minScore)
            {
                minScore = item.Score;
            }
        }

        if(maxScore != minScore)
        {
            foreach(EnvQueryItem item in _envQueryItems)
            {
                item.Score = (item.Score - minScore) / (maxScore - minScore);
            }
        }
    }
    
    private void ResetScore()
    {
        foreach(EnvQueryItem item in _envQueryItems)
        {
            item.IsValid = true;
            item.Score = 0.0f;
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(isActiveSystem == false)return;
        if(isActiveAndEnabled && _envQueryItems != null)
        {
            foreach(EnvQueryItem item in _envQueryItems)
            {
                if(item.IsValid)
                {
                    Gizmos.color = Color.HSVToRGB((item.Score/2.0f), 1.0f, 1.0f);
                    Gizmos.DrawWireSphere(item.GetWorldPosition(), 0.25f);
                    UnityEditor.Handles.Label(item.GetWorldPosition(), item.Score.ToString());
                }
            }
        }
    
        // if(BestResults.Count == 0 || BestResults == null) return;
        // foreach (EnvQueryItem result in BestResults)
        // {
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawSphere(result.GetWorldPosition(), 0.25f);
        // }
        // if(isActiveAndEnabled && BestResult != null)
        // {
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawSphere(BestResult.GetWorldPosition(), 0.25f);
        // }
    }
#endif
}
