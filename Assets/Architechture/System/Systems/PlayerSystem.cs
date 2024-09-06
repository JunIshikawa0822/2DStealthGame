using UnityEngine;
public class PlayerSystem : ASystem, IOnUpdate, IOnFixedUpdate
{
    private IPlayer _player;
    public override void OnSetUp()
    {
        _player = gameStat.player;

        IGun gun = gameStat.Pistol1;

        gun.OnSetUp(gameStat.bullet_10mm_Factories, gameStat.objectPool);
        _player.SetEquipment(gun, 0);
    }

    public void OnUpdate()
    {
        if(gameStat.onAttack)
        {
            _player.OnAttack();
        }
    }

    public  void OnFixedUpdate()
    {
        _player.OnMove(gameStat.moveDirection);
    }
}
