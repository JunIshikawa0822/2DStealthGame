using UnityEngine;
public class PlayerSystem : ASystem, IOnUpdate, IOnFixedUpdate
{
    private APlayer _player;
    public override void OnSetUp()
    {
        _player = gameStat.player;
        _player.OnSetUp(100);

        gameStat.onPlayerAttackEvent += _player.OnAttack;
    }

    public void OnUpdate()
    {
        
    }

    public void OnFixedUpdate()
    {
        _player.OnMove(gameStat.moveDirection, gameStat.cursorWorldPosition);
    }
}
