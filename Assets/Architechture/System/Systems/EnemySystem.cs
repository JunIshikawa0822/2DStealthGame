
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

#region やっつけ
    public void EnemyObjectInstantiate(Vector3 pos)
    {
        // Debug.Log("つくった");
        #region 銃の割り当て
        Enemy_Bandit_Controller enemy = GameObject.Instantiate(gameStat.bandit, pos, Quaternion.identity);
        //IGun gun = GameObject.Instantiate(gameStat.Pistol1);

        //gun.Reload(new Entity_Magazine(10, 10));
        //gun.OnSetUp(gameStat.bullet_10mm_ObjectPool);
        
        #endregion
        
        enemy.OnSetUp(new Entity_HealthPoint(100, 100));

        // Debug.Log(enemy.Storage.WeaponArray[0]);

        IInventoryItem[] weaponDataArray = enemy.EnemyWeaponStorage.GetItems();

        if(weaponDataArray == null || weaponDataArray[0] == null)
        {
            Debug.LogWarning("武器のデータを入れてください");

            //武器のデータを入れる処理
            return;
        }

        enemy.Equip(gameStat.gunFacade.GetGunInstance(weaponDataArray[0].Data));
        enemy.transform.SetParent(gameStat.enemyParent);
    }
#endregion
}
