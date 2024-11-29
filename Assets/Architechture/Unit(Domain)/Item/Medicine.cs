
public class Medicine
{
    AEntity _entity;
    string _medicineName;
    public Medicine(Medicine_Data data, AEntity entity)
    {
        _entity = entity;
        _medicineName = data.ItemName;
    }

    public void ItemUse()
    {
        UnityEngine.Debug.Log($"{_medicineName}をつかったよ");
    }
}
