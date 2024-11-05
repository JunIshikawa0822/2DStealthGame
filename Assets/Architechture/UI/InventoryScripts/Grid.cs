using UnityEngine;
using System;

public class Grid<T> where T : class
{
    private int gridWidth;
    private int gridHeight;   
    private float gridCellSize;
    //public Vector2 gridOriginPosition;
    public T[,] gridArray;

    //Func<Grid<T>, int, int, T> createCellObjectは、Grid<T>, int, intをもらってTを返す処理自体が入る
    public Grid(int width, int height, float cellSize, Func<Grid<T>, int, int, T> createCellObject) 
    {
        gridWidth = width;
        gridHeight = height;
        gridCellSize = cellSize;
        //this.gridOriginPosition = originPosition;
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

    //正しい座標、ではなく、正しい範囲にいるかどうか
    public bool IsValidCellNum(CellNumber cellNum) 
    {
        int x = cellNum.x;
        int y = cellNum.y;

        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight) 
        {
            return true;
        } 
        else 
        {
            return false;
        }
    }

    // public T GetCellObject(int x, int y) 
    // {
    //     if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight) 
    //     {
    //         return gridArray[x, y];
    //     } 
    //     else 
    //     {
    //         return default(T);
    //     }
    // }

    public T GetCellObject(CellNumber cellNum)
    {
        if(cellNum == null)
        {
            Debug.Log("cellNumがnullのため情報が取れないよ");
            return default(T);
        }

        if (cellNum.x >= 0 && cellNum.y >= 0 && cellNum.x < gridWidth && cellNum.y < gridHeight) 
        {
            return gridArray[cellNum.x, cellNum.y];
        } 
        else 
        {
            Debug.Log($"{cellNum.ToString()}は枠外");
            return default(T);
        }
    }

#region UnityEngineを使いたくない
    public CellNumber GetCellNum(Vector2 point)
    {
        //int x = Mathf.FloorToInt((gridOriginPosition.x - position.x) / gridCellSize);
        //int y = Mathf.FloorToInt((gridOriginPosition.y - position.y) / gridCellSize);

        //Debug.Log("計算");
        int x = Mathf.FloorToInt((point.x) / gridCellSize);
        int y = -Mathf.FloorToInt((point.y + gridCellSize) / gridCellSize);

        //Debug.Log(x + "," + y);
        return new CellNumber(x, y);
    }

    public Vector2 GetCellOriginAnchoredPosition(int cellNum_x, int cellNum_y)
    {
        return /*gridOriginPosition + */ new Vector2(cellNum_x, -cellNum_y) * gridCellSize;
    } 
#endregion

}
