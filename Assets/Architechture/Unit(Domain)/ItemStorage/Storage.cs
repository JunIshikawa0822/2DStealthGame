using System.Collections.Generic;

public class Storage
{
    private List<IItem> storageList;
    public Storage()
    {
        storageList = new List<IItem>();
    }

    public void AddItem(IItem item)
    {
        storageList.Add(item);
    }

    public void TakeItem(IItem item)
    {
        if(storageList.IndexOf(item) >= 0)
        {
            
        }
    }
}
