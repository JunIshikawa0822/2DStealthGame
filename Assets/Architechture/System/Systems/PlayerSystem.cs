using UnityEngine;
public class PlayerSystem : ASystem, IOnUpdate, IOnFixedUpdate
{
    private APlayer _player;
    public override void OnSetUp()
    {
        _player = gameStat.player;
        _player.OnSetUp(100);

        //IGun gun = gameStat.Pistol1;

        //gun.OnSetUp(gameStat.bullet_10mm_Factories, gameStat.objectPool_10mm);
        //_player.SetEquipment(gun, 0);

        gameStat.onPlayerAttackEvent += _player.OnAttack;
    }

    public void OnUpdate()
    {
        gameStat.cursorObject.transform.position = gameStat.cursorWorldPosition;
    }

    public void OnFixedUpdate()
    {
        _player.OnMove(gameStat.moveDirection, gameStat.cursorWorldPosition);
    }
}
