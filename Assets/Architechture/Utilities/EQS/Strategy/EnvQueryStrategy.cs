using UnityEngine;
using System;
using System.Collections.Generic;
public abstract class EnvQueryStrategy : ScriptableObject
{
    public bool IsActive = true;
    public float Weight;
    public EnvQueryScoringMode ScoringMode;
    public enum EnvQueryScoringMode
    {
        Linear,
        InverseLinear,
        Square,
        HalfSine,
        InverseHalfSine,
        HalfSineSquared,
        InverseHalfSineSquared,
        SigmoidLike,
        InverseSigmoidLike
    }
    public abstract void RunStrategy(int currentStrategyIndex, List<EnvQueryItem> items);
    public void NormalizeItemScores(int currentTest, List<EnvQueryItem> envQueryItems)
    {
        if(envQueryItems == null || envQueryItems.Count < 1)
        {
            return;
        }

		float maxValue = envQueryItems[0].TestResults[currentTest];
		float minValue = envQueryItems[0].TestResults[currentTest];

		foreach(EnvQueryItem item in envQueryItems)
		{
            if(item.IsValid)
            {
                float value = item.TestResults[currentTest];
                if(value > maxValue)
                {
                    maxValue = value;
                }
                if(value < minValue)
                {
                    minValue = value;
                }
            }
		}

        if(maxValue != minValue)
        {
            foreach(EnvQueryItem item in envQueryItems)
            {
                if(!item.IsValid)
                {
                    continue;
                }

                float weightedScore = 0.0f;
                float normalizedScore = (item.TestResults[currentTest] - minValue) / (maxValue - minValue);

                switch(ScoringMode)
                {
                    case EnvQueryScoringMode.Linear:
                        weightedScore = Weight * normalizedScore;
                        break;
                    case EnvQueryScoringMode.InverseLinear:
                        weightedScore = Weight * (1.0f - normalizedScore);
                        break;
                    case EnvQueryScoringMode.Square:
                        weightedScore = Weight * (normalizedScore * normalizedScore);
                        break;
                    case EnvQueryScoringMode.HalfSine:
                        weightedScore = Weight * Mathf.Sin(Mathf.PI * normalizedScore);
                        break;
                    case EnvQueryScoringMode.InverseHalfSine:
                        weightedScore = Weight * -Mathf.Sin(Mathf.PI * normalizedScore);
                        break;
                    case EnvQueryScoringMode.HalfSineSquared:
                        weightedScore = Weight * Mathf.Sin(Mathf.PI * normalizedScore) * Mathf.Sin(Mathf.PI * normalizedScore);
                        break;
                    case EnvQueryScoringMode.InverseHalfSineSquared:
                        weightedScore = Weight * -(Mathf.Sin(Mathf.PI * normalizedScore) * Mathf.Sin(Mathf.PI * normalizedScore));
                        break;
                    case EnvQueryScoringMode.SigmoidLike:
                        weightedScore = Weight * (((float)Math.Tanh( 4.0f * (normalizedScore - 0.5f) ) + 1.0f) / 2.0f);
                        break;
                    case EnvQueryScoringMode.InverseSigmoidLike:
                        weightedScore = Weight * ( 1.0f - (((float)Math.Tanh( 4.0f * (normalizedScore - 0.5f) ) + 1.0f) / 2.0f) );
                        break;
                    default:
                        break;
                }

                item.Score += weightedScore;
            }
        }
    }
}
