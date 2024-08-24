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

        //Debug.Log(gridOriginPosition);
    }

    //正しい座標、ではなく、正しい範囲にいるかどうか
    public bool IsValidCellNum(Vector2Int gridPosition) {
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

    public Vector2Int GetCellNum(/*Vector2 position*/ Vector2 anchoredPosition)
    {
        //int x = Mathf.FloorToInt((gridOriginPosition.x - position.x) / gridCellSize);
        //int y = Mathf.FloorToInt((gridOriginPosition.y - position.y) / gridCellSize);

        int x = Mathf.FloorToInt((anchoredPosition.x) / gridCellSize);
        int y = Mathf.FloorToInt((anchoredPosition.y) / gridCellSize);

        //Debug.Log(x + "," + y);
        return new Vector2Int(x, y);
    }

    public Vector2 GetCellOriginAnchoredPosition(int cellNum_x, int cellNum_y)
    {
        return /*gridOriginPosition + */ new Vector2(cellNum_x, cellNum_y) * gridCellSize;
    }  
}
