using UnityEngine;
using System.Diagnostics;

public class ItemSystem : ASystem, IOnUpdate
{
    //Storage playerStorage;
    public override void OnSetUp()
    {
        gameStat.playerStorage = new Storage();

        gameStat.playerStorage.AddItem(ItemMake(gameStat.handgunDataArray[0], 1, new CellNumber(0, 0)));
        gameStat.playerStorage.AddItem(ItemMake(gameStat.shotgunDataArray[0], 1, new CellNumber(0, 2)));
        gameStat.playerStorage.AddItem(ItemMake(gameStat.rifleDataArray[0], 1, new CellNumber(0, 4)));
        gameStat.playerStorage.AddItem(ItemMake(gameStat.handgunDataArray[0], 1, new CellNumber(0, 6)));
        gameStat.playerStorage.AddItem(ItemMake(gameStat.medicineDataArray[0], 2, new CellNumber(0, 8)));

        gameStat.inventory1.OpenInventory(gameStat.playerStorage);
        
    }

    public ItemData ItemMake(A_Item_Data data, uint num, CellNumber place)
    {
        ItemData item = new ItemData(data, num);
        item.SetPosDir(place, ItemData.ItemDir.Down);
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

    public void PointerDown(GUI_Item item)
    {
        UnityEngine.Debug.Log("クリックした！！");
    }

    public void StartDragging(GUI_Item item)
    {
        UnityEngine.Debug.Log("マウス開始");
    }

    public void EndDragging(GUI_Item item)
    {
        UnityEngine.Debug.Log("マウス終了");
    }


}
