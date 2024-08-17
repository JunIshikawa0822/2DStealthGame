using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid<T>
{
    public int gridWidth;
    public int gridHeight;
    public float gridCellSize;
    public Vector2 originPosition;
    private T[,] gridArray;

    //Func<Grid<T>, int, int, T> createCellObjectは、Grid<T>, int, intをもらってTを返す処理自体が入る
    public Grid(int width, int height, float cellSize, Vector2 originPosition, Func<Grid<T>, int, int, T> createCellObject) 
    {
        gridWidth = width;
        gridHeight = height;
        gridCellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new T[width, height];

        //0次元の配列の長さ（gridWidth)
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            //1次元の配列の長さ（gridHeight)
            for (int y = 0; y < gridArray.GetLength(1); y++) 
            {
                gridArray[x, y] = createCellObject(this, x, y);
                Debug.Log(x + ", " + y);
            }
        }

        bool showDebug = true;

        if (showDebug) {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int n = 0; n < gridArray.GetLength(0) + 1; n++) {
                Debug.DrawLine(originPosition + new Vector2(n * cellSize, 0), originPosition + new Vector2(n * cellSize, gridHeight * cellSize), Color.white, 100f);
                Debug.Log((originPosition + new Vector2(n * cellSize, 0)) + "から" + (originPosition + new Vector2(n * cellSize, gridHeight * cellSize)));
            }

            for (int n = 0; n < gridArray.GetLength(1) + 1; n++) {
                Debug.DrawLine(originPosition + new Vector2(0, n * cellSize), originPosition + new Vector2(gridWidth * cellSize, n * cellSize), Color.white, 100f);
                Debug.Log((originPosition + new Vector2(0, n * cellSize)) + "から" + (originPosition + new Vector2(gridWidth * cellSize, n * cellSize)));
            }

            // Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            // Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            // OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
            //     debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            // };
        }
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
        return new Vector3(x, y) * gridCellSize + new Vector3(originPosition.x, originPosition.y, 0);
    }
}
