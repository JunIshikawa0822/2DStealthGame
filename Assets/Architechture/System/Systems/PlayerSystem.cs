public class PlayerSystem : ASystem, IOnUpdate, IOnFixedUpdate
{
    private APlayer _player;
    public override void OnSetUp()
    {
        _player = gameStat.player;
        _player.OnSetUp(100);

        gameStat.onPlayerAttackEvent += OnAttack;
        gameStat.onPlayerReloadEvent += OnReload;
    }

    public void OnUpdate()
    {
        
    }

    public void OnFixedUpdate()
    {
        _player.OnMove(gameStat.moveDirection, gameStat.cursorWorldPosition);
    }

    public void OnAttack()
    {
        _player.OnAttack(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex]);
    }

    public void OnReload()
    {
        Entity_Magazine magazine = new Entity_Magazine(10, 10);
        _player.OnReload(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex], magazine);
    }
}
