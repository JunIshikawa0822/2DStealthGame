using UnityEngine;
using UnityEngine.EventSystems;
public class PlacedObject : ItemDragAndDrop
{
    public Dir direction;
    public int width;
    public int height;

    public RectTransform rectTransform;

    void OnSetUp()
    {
        rectTransform = GetComponent<RectTransform>();
    } 

    public enum Dir {
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

    public Vector2Int GetRotationOffset(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:  return new Vector2Int(0, 0);
            case Dir.Left:  return new Vector2Int(0, width);
            case Dir.Up:    return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }

    public Dir GetDir()
    {
        return direction;
    }
}
