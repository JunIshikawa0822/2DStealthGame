using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Scriptable_UI_ItemData", menuName = "ScriptableObject/UI_Item", order = 0)]
public class Scriptable_UI_Item : ScriptableObject
{
    public string nameString;
    public Transform prefab;
    public Transform backGroundPrefab;
    public int width;
    public int height;

    //public ItemDirection.Dir direction;

    public ItemDir direction;

    public int stackableNum;

    public enum ItemDir
    {
        Down,
        Left,
        Up,
        Right,
    }

    public ItemDir GetNextDir(ItemDir dir) 
    {
        switch (dir) {
            default:
            case ItemDir.Down:      return ItemDir.Left;
            case ItemDir.Left:      return ItemDir.Up;
            case ItemDir.Up:        return ItemDir.Right;
            case ItemDir.Right:     return ItemDir.Down;
        }
    }

    public int GetRotationAngle(ItemDir itemDirection)
    {
        switch (itemDirection) 
        {
            default:
            case ItemDir.Down :  return 0;
            case ItemDir.Left:  return 90;
            case ItemDir.Up:    return 180;
            case ItemDir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(ItemDir itemDirection) 
    {
        switch (itemDirection)
        {
            default:
            case ItemDir.Down:  return new Vector2Int(0, 0);
            case ItemDir.Left:  return new Vector2Int(height, 0);
            case ItemDir.Up:    return new Vector2Int(width, height);
            case ItemDir.Right: return new Vector2Int(0, height);
        }
    }

    // public Vector2Int GetDragCellNumRotateOffset(ItemDir itemDirection, Vector2Int offset)
    // {
    //     switch (itemDirection)
    //     {
    //         default:
    //         case ItemDir.Down:  return new Vector2Int(offset.x, offset.y);
    //         case ItemDir.Left:  return new Vector2Int(0, offset.x);
    //         case ItemDir.Up:    return new Vector2Int(offset.x, 0);
    //         case ItemDir.Right: return new Vector2Int(offset.y, offset.x);
    //     }
    // }

    public Vector2Int GetCellNumRotateOffset(ItemDir originDirection, ItemDir itemDirection, Vector2Int offset)
    {
        Debug.Log("originDir : " + originDirection + " , itemDir : " + itemDirection);
        int rest_x = (width - 1) - offset.x;
        int rest_y = (height - 1) - offset.y;

        Vector2Int rotateOffset = new Vector2Int(offset.x, offset.y);

        if(itemDirection == originDirection)
        {
            rotateOffset = new Vector2Int(offset.x, offset.y);
        }
        else if(itemDirection == GetNextDir(originDirection))
        {
            rotateOffset = new Vector2Int(rest_y, offset.x);
        }
        else if(itemDirection == GetNextDir(GetNextDir(originDirection)))
        {
            rotateOffset = new Vector2Int(rest_x, rest_y);
        }
        else
        {
            rotateOffset = new Vector2Int(offset.y, rest_x);
        }

        Debug.Log("offset : " + rotateOffset);
        return rotateOffset;
    }

    public List<Vector2Int> GetCellNumList(ItemDir itemDirection, Vector2Int originCellNum) 
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        switch (itemDirection) 
        {
            default:
            case ItemDir.Down:
            case ItemDir.Up:
                for (int x = 0; x < width; x++) 
                {
                    for (int y = 0; y < height; y++) 
                    {
                        gridPositionList.Add(originCellNum + new Vector2Int(x, y));
                    }
                }
                break;
            case ItemDir.Left:
            case ItemDir.Right:
                for (int x = 0; x < height; x++) 
                {
                    for (int y = 0; y < width; y++) 
                    {
                        gridPositionList.Add(originCellNum + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }
}
