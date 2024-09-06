using UnityEngine;
public class PlayerSystem : ASystem, IOnUpdate, IOnFixedUpdate
{
    private APlayer _player;

    private IEntity entity;
    public override void OnSetUp()
    {
        _player = gameStat.player;
        entity = _player;

        _player.OnSetUp(100);

        IGun gun = gameStat.Pistol1;

        gun.OnSetUp(gameStat.bullet_10mm_Factories, gameStat.objectPool);
        _player.SetEquipment(gun, 0);

        entity.OnEntityDead();
    }

    public void OnUpdate()
    {
        if(gameStat.onAttack)
        {
            _player.OnAttack();
        }
    }

    public void OnFixedUpdate()
    {
        _player.OnMove(gameStat.moveDirection);
    }
}
