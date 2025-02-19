
using UnityEngine;

public class EnemySystem : ASystem, IOnUpdate
{
    public override void OnSetUp()
    {
        if(gameStat.enemyObjects.Count < 1) return;
        foreach(AEnemy enemy in gameStat.enemyObjects)
        {
            enemy.OnSetUp(new Entity_HealthPoint(100, 100));
            EquipGun(enemy);
            enemy.onEnemyDeadEvent += (AGun gun) => gameStat.gunFacade.ReturnGunInstance(gun);
        }
    }
    public void OnUpdate()
    {
        
    }

    public void EquipGun(AEnemy enemy)
    {
        IInventoryItem[] weaponDataArray = enemy.WeaponStorage.GetItems();

        if(weaponDataArray == null || weaponDataArray[0] == null)
        {
            Debug.LogWarning("武器のデータを入れてください");

            //武器のデータを入れる処理
            return;
        }

        I_Data_Item weaponData = weaponDataArray[0].Data;

        enemy.Equip(gameStat.gunFacade.GetGunInstance(weaponData));
    }
    
    
}
