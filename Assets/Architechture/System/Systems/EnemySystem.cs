
using UnityEngine;

public class EnemySystem : ASystem, IOnUpdate
{
    public override void OnSetUp()
    {
        EnemyObjectInstantiate(new Vector3(-10, 0.5f, 25));
        EnemyObjectInstantiate(new Vector3(25,0.5f,1.7f));
        EnemyObjectInstantiate(new Vector3(15,0.5f,-15));
        EnemyObjectInstantiate(new Vector3(17,0.5f,22));
        EnemyObjectInstantiate(new Vector3(0.8f,0.5f,6.4f));
    }
    public void OnUpdate()
    {
        
    }

#region このままでいいのかな
    public void EnemyObjectInstantiate(Vector3 pos)
    {
        // Debug.Log("つくった");
        #region 銃の割り当て
        Enemy_Bandit_Controller enemy = GameObject.Instantiate(gameStat.bandit, pos, Quaternion.identity);
        //IGun gun = GameObject.Instantiate(gameStat.Pistol1);
        #endregion
        
        enemy.OnSetUp(new Entity_HealthPoint(100, 100));

        Debug.Log(enemy.WeaponStorage);
        
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
#endregion
}
