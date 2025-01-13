using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStorage
{
    public void Add(IInventoryItem data);
    public void Remove(IInventoryItem data);

    public IInventoryItem[] GetItems();
}
