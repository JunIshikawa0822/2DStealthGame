using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UI_Item", menuName = "ScriptableObject/UI", order = 0)]
public class Scriptable_UI_Item : ScriptableObject
{
    public string nameString;
    public Transform prefab;
    public Transform visual;
    public int width;
    public int height;

    public Dir direction;

    public enum Dir 
    {
        Down,
        Left,
        Up,
        Right,
    }

    public Dir GetNextDir(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:      return Dir.Left;
            case Dir.Left:      return Dir.Up;
            case Dir.Up:        return Dir.Right;
            case Dir.Right:     return Dir.Down;
        }
    }

    public int GetRotationAngle(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:  return 0;
            case Dir.Left:  return 90;
            case Dir.Up:    return 180;
            case Dir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir) {
        switch (dir) 
        {
            default:
            case Dir.Down:  return new Vector2Int(0, 0);
            case Dir.Left:  return new Vector2Int(0, width);
            case Dir.Up:    return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }

    // public List<Vector2Int> GetCellNumList(Vector2Int originCellNum, PlacedObject.Dir dir) 
    // {
    //     List<Vector2Int> gridPositionList = new List<Vector2Int>();

    //     switch (dir) 
    //     {
    //         default:
    //         case PlacedObject.Dir.Down:
    //         case PlacedObject.Dir.Up:
    //             for (int x = 0; x < width; x++) {
    //                 for (int y = 0; y < height; y++) {
    //                     gridPositionList.Add(originCellNum + new Vector2Int(x, y));
    //                 }
    //             }
    //             break;
    //         case PlacedObject.Dir.Left:
    //         case PlacedObject.Dir.Right:
    //             for (int x = 0; x < height; x++) {
    //                 for (int y = 0; y < width; y++) {
    //                     gridPositionList.Add(originCellNum + new Vector2Int(x, y));
    //                 }
    //             }
    //             break;
    //     }
    //     return gridPositionList;
    // }
}
