using UnityEngine;
using UnityEngine.EventSystems;

public class PlacedObject : ItemDragAndDrop<PlacedObject>
{
    [System.NonSerialized]
    public Dir direction;

    [System.NonSerialized]
    public int width;

    [System.NonSerialized]
    public int height;

    [System.NonSerialized]
    public RectTransform rectTransform;

    [System.NonSerialized]
    public PlacedObject placedObject;

    public TetrisInventory belongingInventory;

    public Vector2Int belongingCellNum;

    public void OnSetUp()
    {
        rectTransform = GetComponent<RectTransform>();
        placedObject = GetComponent<PlacedObject>();
        this.direction = Dir.Down;
        Tobject = placedObject;
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
