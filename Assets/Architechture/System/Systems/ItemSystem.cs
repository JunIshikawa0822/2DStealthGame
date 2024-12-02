
public class ItemSystem : ASystem, IOnUpdate
{
    Storage playerStorage;
    public override void OnSetUp()
    {
        playerStorage = new Storage();

        playerStorage.AddItem(ItemMake(gameStat.handgunDataArray[0], 1, new CellNumber(0, 0)));
        playerStorage.AddItem(ItemMake(gameStat.shotgunDataArray[0], 1, new CellNumber(0, 2)));
        playerStorage.AddItem(ItemMake(gameStat.rifleDataArray[0], 1, new CellNumber(0, 4)));
        playerStorage.AddItem(ItemMake(gameStat.handgunDataArray[0], 1, new CellNumber(0, 6)));
        playerStorage.AddItem(ItemMake(gameStat.medicineDataArray[0], 1, new CellNumber(0, 8)));

        gameStat.inventory1.OpenInventory(playerStorage);
    }

    public ItemData ItemMake(A_Item_Data data, uint num, CellNumber place)
    {
        ItemData item = new ItemData(data, num);
        item.SetPosDir(place, ItemData.ItemDir.Down);
        return item;
    }

    public void OnUpdate()
    {

    }


}
