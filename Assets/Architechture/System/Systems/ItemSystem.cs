using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

public class ItemSystem : ASystem, IOnUpdate
{
    Dictionary<int, IItem> itemInstanceArray;

    #region DataArray
    A_Item_Data[][] _dataArray;
    #endregion

    public override void OnSetUp()
    {
        gameStat.playerStorage = gameStat.player.GetComponent<Storage>();

        gameStat.playerStorage.AddItem(gameStat.playerStorage.ItemMake(gameStat.handgunDataArray[0], 1, new CellNumber(0, 0)));
        gameStat.playerStorage.AddItem(gameStat.playerStorage.ItemMake(gameStat.shotgunDataArray[0], 1, new CellNumber(0, 2)));
        gameStat.playerStorage.AddItem(gameStat.playerStorage.ItemMake(gameStat.rifleDataArray[0], 1, new CellNumber(0, 4)));
        gameStat.playerStorage.AddItem(gameStat.playerStorage.ItemMake(gameStat.handgunDataArray[0], 1, new CellNumber(0, 6)));
        gameStat.playerStorage.AddItem(gameStat.playerStorage.ItemMake(gameStat.medicineDataArray[0], 2, new CellNumber(0, 8)));

        //全てのデータをもとに、それぞれ一つのインスタンスを作成。IDとインスタンスをセットにして保存
        itemInstanceArray = new[]
        { 
            gameStat.foodDataArray.ToDictionary(data => data.ItemID, data => new Food(data) as IItem), 
            gameStat.medicineDataArray.ToDictionary(data => data.ItemID, data => new Medicine(data) as IItem),
            gameStat.handgunDataArray.ToDictionary(data => data.ItemID, data => gameStat.gunFactoriesList[0].GunInstantiate(data) as IItem),
            // gameStat.rifleDataArray.ToDictionary(data => data.ItemID, data => gameStat.gunFactories.GunInstantiate(data) as IItem),
            // gameStat.shotgunDataArray.ToDictionary(data => data.ItemID, data => gameStat.gunFactories.GunInstantiate(data) as IItem),
            // gameStat.subMachinegunDataArray.ToDictionary(data => data.ItemID, data => gameStat.gunFactories.GunInstantiate(data) as IItem)
        }
        .SelectMany(dict => dict)
        .GroupBy(pair => pair.Key) // キーでグループ化
        .ToDictionary(
            group => group.Key, // キーはそのまま
            group => group.Last().Value // 最後の値を選ぶ
        );

        gameStat.itemFacade = new ItemFacade(itemInstanceArray);
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

        if(Input.GetKeyDown(KeyCode.N))
        {
            gameStat.itemFacade.CheckRef(100);
        }
    }


}
