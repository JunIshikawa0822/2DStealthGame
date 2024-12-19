using UnityEngine;
public class PlayerSystem : ASystem, IOnUpdate, IOnFixedUpdate, IOnLateUpdate
{
    private PlayerController _player;
    public override void OnSetUp()
    {
        _player = gameStat.player;

        DrawFieldOfView playerFieldOfView = new DrawFieldOfView
        (
            gameStat.meshFilter, 
            new Mesh(), 
            gameStat.targetLayer, 
            gameStat.obstacleLayer, 
            gameStat.meshResolution, 
            gameStat.edgeResolveIterations,
            gameStat.edgeDstThreshold
        );

        FindOpponent findOpponent = new FindOpponent(gameStat.targetLayer, gameStat.obstacleLayer);
        DrawOpponent drawOpponent = new DrawOpponent();
        Entity_HealthPoint playerHP = new Entity_HealthPoint(100, 100);

        _player.OnSetUp(playerHP, playerFieldOfView, findOpponent, drawOpponent, gameStat.radius, gameStat.angle);
        _player.storageFindEvent += OnFindStorage;
        _player.leaveStorageEvent += OnExitStorage;

        gameStat.onPlayerAttackEvent += OnAttack;
        gameStat.onPlayerReloadEvent += OnReload;
        gameStat.onEquipEvent += OnEquipGun;
        gameStat.onUnEquipEvent += OnUnEquipGun;
    }

    public void OnUpdate()
    {
        if(gameStat.isInventoryPanelActive)return;

        // if(_player == null) 
        // {
        //     Debug.Log(_player);
        //     return;
        // }

        _player.Rotate(gameStat.cursorWorldPosition);
    }

    public void OnLateUpdate()
    {
        _player.DrawView();
    }

    public void OnFixedUpdate()
    {
        if(gameStat.isInventoryPanelActive)return;
        
        //画面上のプレイヤー移動方向と入力方向が異なってみえる　45度回転することで解決
        float x = (gameStat.moveDirection.x) * Mathf.Cos(-45 * Mathf.Deg2Rad) - (gameStat.moveDirection.y) * Mathf.Sin(-45 * Mathf.Deg2Rad);
        float z = (gameStat.moveDirection.x) * Mathf.Sin(-45 * Mathf.Deg2Rad) + (gameStat.moveDirection.y) * Mathf.Cos(-45 * Mathf.Deg2Rad);
        Vector2 vector = new Vector3(x, z);

        if(vector == Vector2.zero)return;
        _player.Move(vector);
    }

    public void OnAttack()
    {
        if(gameStat.isInventoryPanelActive)return;

        if(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex] == null)return;
        _player.Attack(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex]);
    }

    public void OnReload()
    {
        if(gameStat.isInventoryPanelActive)return;
        Entity_Magazine magazine = new Entity_Magazine(10, 10);
        _player.Reload(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex], magazine);
    }

    public void OnEquipGun(int index, ItemData data)
    {
        IGunData gunData = data.ObjectData as IGunData;
        if(gunData == null) return;

        AGun gun = gameStat.gunFacade.GetGunInstance(gunData);
        gameStat.playerGunsArray[index] = gun;

        _player.Equip(gun);
    }

    public void OnUnEquipGun(int index)
    {
        //gameStat.playerGunsArray[index].gameObject.SetActive(false);
        gameStat.gunFacade.ReturnGunInstance(gameStat.playerGunsArray[index]);
        gameStat.playerGunsArray[index] = null;
    }

    public void OnFindStorage(Storage storage)
    {
        gameStat.otherStorage = storage;
    }

    public void OnExitStorage(Storage storage)
    {
        gameStat.otherStorage = null;
    }
}
