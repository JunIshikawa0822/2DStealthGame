using UnityEngine;
using System.Diagnostics;

public class ItemSystem : ASystem, IOnUpdate
{
    //Storage playerStorage;
    public override void OnSetUp()
    {
        gameStat.playerStorage = gameStat.player.GetComponent<Storage>();

        gameStat.playerStorage.AddItem(ItemMake(gameStat.handgunDataArray[0], 1, new CellNumber(0, 0)));
        gameStat.playerStorage.AddItem(ItemMake(gameStat.shotgunDataArray[0], 1, new CellNumber(0, 2)));
        gameStat.playerStorage.AddItem(ItemMake(gameStat.rifleDataArray[0], 1, new CellNumber(0, 4)));
        gameStat.playerStorage.AddItem(ItemMake(gameStat.handgunDataArray[0], 1, new CellNumber(0, 6)));
        gameStat.playerStorage.AddItem(ItemMake(gameStat.medicineDataArray[0], 2, new CellNumber(0, 8)));
    }

    public ItemData ItemMake(A_Item_Data data, uint num, CellNumber place)
    {
        ItemData item = new ItemData(data, num);
        item.Address = place;
        item.Direction = ItemData.ItemDir.Down;
        return item;
    }

    public void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            gameStat.inventory1.OpenInventory(gameStat.playerStorage);
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            gameStat.inventory1.CloseInventory();
        }
    }


}
