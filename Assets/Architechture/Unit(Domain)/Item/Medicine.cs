
public class Medicine : IObject
{
    public string Name{get;set;}
    public Medicine(Medicine_Data data)
    {
        Name = data.ItemName;
    }

    public void ItemUse()
    {
        UnityEngine.Debug.Log($"{Name}をつかったよ");
    }
}
