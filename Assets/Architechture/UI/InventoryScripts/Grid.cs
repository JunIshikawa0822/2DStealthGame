using UnityEngine;
using System;

public class Grid<T>
{
    public int gridWidth;
    public int gridHeight;   
    public float gridCellSize;
    public Vector2 gridOriginPosition;
    public T[,] gridArray;

    //Func<Grid<T>, int, int, T> createCellObjectは、Grid<T>, int, intをもらってTを返す処理自体が入る
    public Grid(int width, int height, float cellSize, Vector2 originPosition, Func<Grid<T>, int, int, T> createCellObject) 
    {
        gridWidth = width;
        gridHeight = height;
        gridCellSize = cellSize;
        this.gridOriginPosition = originPosition;
        gridArray = new T[width, height];

        //0次元の配列の長さ（gridWidth)
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            //1次元の配列の長さ（gridHeight)
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createCellObject(this, x, y);
            }
        }
    }

    

    public Vector3 GetWorldPositionFromRectPosition(Vector2 pos, RectTransform rect)
    {
        //UI座標からスクリーン座標に変換
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, pos);
        //ワールド座標
        Vector3 result = Vector3.zero;
        //スクリーン座標→ワールド座標に変換
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPos, null, out result);
        return result;
    }

    public bool IsValidGridPosition(Vector2Int gridPosition) {
        int x = gridPosition.x;
        int y = gridPosition.y;

        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight) 
        {
            return true;
        } 
        else 
        {
            return false;
        }
    }

    public T GetGridObject(int x, int y) {
        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight) 
        {
            return gridArray[x, y];
        } 
        else 
        {
            return default(T);
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(gridOriginPosition.x, gridOriginPosition.y, 0) + new Vector3(x, y) * gridCellSize;
    }
}
