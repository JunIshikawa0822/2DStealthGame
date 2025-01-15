using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;

public class NormalStorage : MonoBehaviour, IStorage
{
    List<IInventoryItem> _itemList = new List<IInventoryItem>();
    [SerializeField]private List<ScriptableObject> _testDataList = new List<ScriptableObject>();

    void Start()
    {
        TestDataLoad(_testDataList, 10, 10);

        // foreach(IInventoryItem item in _itemList)
        // {
        //     Debug.Log(item.Address + " , Width : " + item.Data.Width + " , Height : " + item.Data.Height);
        // }
    }

    public void Add(IInventoryItem item)
    {
        _itemList.Add(item);
    }

    public void Remove(IInventoryItem item)
    {
        int num = _itemList.IndexOf(item);

        if(num > 0)
        {
            _itemList.RemoveAt(num);
        }
        else
        {
            Debug.Log("該当のデータが存在しません");
        }
    }

    public IInventoryItem[] GetItems()
    {
        return _itemList.ToArray();
    }

    private void TestDataLoad(List<ScriptableObject> testDataList, int width, int height)
    {
        int[,] vInv = new int[width, height];

        for(int i = 0; i < testDataList.Count; i++)
        {
            if(!(testDataList[i] is I_Data_Item data))return;

            (int x, int y) canPlacePos = CanPlaceObject(vInv, data.Width, data.Height);

            if(canPlacePos.x > -1 && canPlacePos.y > -1)
            {
                PlaceObject(vInv, canPlacePos.x, canPlacePos.y, data.Width, data.Height);
                
                IInventoryItem newItem = new InventoryItem(data, 1);
                newItem.Address = new CellNumber(canPlacePos.x, canPlacePos.y);

                Add(newItem);
            }
            else
            {
                canPlacePos = CanPlaceObject(vInv, data.Height, data.Width);

                if(canPlacePos.x > -1 && canPlacePos.y > -1)
                {
                    PlaceObject(vInv, canPlacePos.x, canPlacePos.y, data.Width, data.Height);
                
                    IInventoryItem newItem = new InventoryItem(data, 1);
                    newItem.Address = new CellNumber(canPlacePos.x, canPlacePos.y);

                    Add(newItem);
                }
                else
                {
                    Debug.Log("入れられない");
                }
            }
        }
    }

    private void PlaceObject(int[,] grid, int startX, int startY, uint dataW, uint dataH)
    {
        for (int dy = 0; dy < dataH; dy++)
        {
            for (int dx = 0; dx < dataW; dx++)
            {
                grid[startY + dy, startX + dx] = 1;
            }
        }
    }

    // オブジェクトを配置するメソッド
    private (int Ax, int Ay) CanPlaceObject(int[,] grid, uint dataW, uint dataH)
    {
        int gridHeight = grid.GetLength(0);
        int gridWidth = grid.GetLength(1);

        for (int y = 0; y <= gridHeight - dataH; y++)
        {
            for (int x = 0; x <= gridWidth - dataW; x++)
            {
                if (CanPlaceCell(grid, dataH, dataW, x, y))
                {
                    return (x, y);
                }
            }
        }
        return (-1, -1); // 配置できない
    }

    #region バグあり
    // 指定した位置にオブジェクトを配置できるか確認するメソッド
    bool CanPlaceCell(int[,] grid, uint dataW, uint dataH, int startX, int startY)
    {
        for (int dy = 0; dy < dataH; dy++)
        {
            for (int dx = 0; dx < dataW; dx++)
            {
                if (grid[startY + dy, startX + dx] != 0)
                {
                    return false; // 他のオブジェクトと重なる場合
                }
            }
        }
        return true;
    }

    #endregion
}
