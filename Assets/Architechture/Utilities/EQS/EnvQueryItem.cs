using UnityEngine;

public class EnvQueryItem
{
    public float Score;
    public bool IsValid;
    public float[] TestResults;

    private Transform centerOfItems; // 基準位置
    private Vector3 location; // 相対位置
        
    public EnvQueryItem(int numTests, Vector3 location, Transform centerOfItems)
    {
        Score = 0.0f;
        IsValid = true;
        TestResults = new float[numTests];
        this.centerOfItems = centerOfItems;
        this.location = location;
    }

    public Vector3 GetWorldPosition()
    {
        return centerOfItems.position + location;
    }
}
