using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateGrid : IGeneratorBase
{
    private float _radius;
    private float _spaceBetween;
    
    public GenerateGrid(float radius, float spaceBetween)
    {
        _radius = radius;
        _spaceBetween = spaceBetween;
    }

    //グリッド状に展開
    public List<EnvQueryItem> GenerateItems(int numberOfStrategy, Transform centerOfItems)
    {
        List<EnvQueryItem> items = new List<EnvQueryItem>();
    
        // 中心点を追加
        Vector3 position = Vector3.zero;
        items.Add(new EnvQueryItem(numberOfStrategy, position, centerOfItems));
    
        int numOfSteps = (int)Mathf.Ceil(_radius / _spaceBetween);
    
        Debug.Log(numOfSteps);
        // すべての象限を一度に処理
        for(int xi = -numOfSteps; xi < numOfSteps; xi++)
        {
        	for(int zi = -numOfSteps; zi < numOfSteps; zi++)
        	{
    
        		position.x = xi * _spaceBetween + (_spaceBetween/2.0f);
        		position.y = 0.0f;
        		position.z = zi * _spaceBetween + (_spaceBetween/2.0f);
               
        		Debug.Log(position);
        		
        		items.Add(new EnvQueryItem(numberOfStrategy, position, centerOfItems));
        	}
        }
        
        return items;
    }
}
