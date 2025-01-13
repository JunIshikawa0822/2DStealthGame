using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.Entities.UniversalDelegates;
public class PlayerSystem : ASystem, IOnUpdate, IOnFixedUpdate, IOnLateUpdate
{
    private PlayerController _player;
    //private AGun[] _playerGuns;
    public override void OnSetUp()
    {
        _player = gameStat.player;
        gameStat.playerHP = new Entity_HealthPoint(100, 100);
        //gameStat.observablePlayerHP = new ReactiveProperty<float>(gameStat.playerHP.CurrentHp);

        gameStat.playerStorage = gameStat.player.Storage;
        gameStat.weaponStorages[0] = gameStat.player.PlayerWeaponStorage1;
        gameStat.weaponStorages[1] = gameStat.player.PlayerWeaponStorage2;

        Debug.Log(gameStat.playerStorage);
        Debug.Log(gameStat.weaponStorages[0]);
        Debug.Log(gameStat.weaponStorages[1]);

        _player.OnSetUp(gameStat.playerHP);
        //_player.PlayerSetUp(gameStat.playerGunsArray.Value, gameStat.selectingGunsArrayIndex.Value);

        _player.storageFindEvent += OnFindStorage;
        _player.leaveStorageEvent += OnExitStorage;

        gameStat.onPlayerAttackEvent += OnAttack;
        gameStat.onPlayerReloadEvent += OnReload;
        gameStat.onPlayerEquipEvent += OnEquipGun;
        gameStat.onPlayerUnEquipEvent += OnUnEquipGun;

        SetEvent();
    }

    public void SetEvent()
    {
        gameStat.playerGunsArray.ObserveReplace().Subscribe(x =>
        {
            OnEquip();
        });
    }

    public void OnUpdate()
    {
        if(gameStat.isInventoryPanelActive)
        {

        }
        else
        {
            Vector2 vector = RotateVec(gameStat.moveDirection, -45);
            _player.MoveAnimation(vector);

            if(gameStat.cursorWorldPosition == Vector3.zero)return;
            _player.Rotate(gameStat.cursorWorldPosition);
        }
    }

    public void OnLateUpdate()
    {
        //_player.DrawView();
    }

    public void OnFixedUpdate()
    {
        if(gameStat.isInventoryPanelActive)return;
        
        Vector2 vector = RotateVec(gameStat.moveDirection, -45);
        // Debug.Log(vector);

        if(vector == Vector2.zero)return;

        //_player.Move(vector);
    }

    private Vector2 RotateVec(Vector2 baseVec, float rotateAngle)
    {
        float x = (baseVec.x) * Mathf.Cos(rotateAngle * Mathf.Deg2Rad) - (baseVec.y) * Mathf.Sin(rotateAngle * Mathf.Deg2Rad);
        float z = (baseVec.x) * Mathf.Sin(rotateAngle * Mathf.Deg2Rad) + (baseVec.y) * Mathf.Cos(rotateAngle * Mathf.Deg2Rad);

        return new Vector2(x,z);
    }

    public void OnAttack()
    {
        if(gameStat.isInventoryPanelActive)return;
        _player.Attack(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex.Value]);
    }

    public void OnReload()
    {
        if(gameStat.isInventoryPanelActive)return;
        _player.Reload(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex.Value]);
    }

    public void OnEquip()
    {
        _player.Equip(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex.Value]);
    }

    public void OnEquipGun(int index, I_Data_Item data)
    {
        if(data == null) return;

        AGun gun = gameStat.gunFacade.GetGunInstance(data);
        gameStat.playerGunsArray[index] = gun;
    }

    public void OnUnEquipGun(int index, I_Data_Item data)
    {
        gameStat.gunFacade.ReturnGunInstance(gameStat.playerGunsArray[index]);
        gameStat.playerGunsArray[index] = null;
    }

    public void OnFindStorage(IStorage storage)
    {
        gameStat.otherStorage = storage;
    }

    public void OnExitStorage(IStorage storage)
    {
        gameStat.otherStorage = null;
    }
}
