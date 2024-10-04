using UnityEngine;
public class PlayerSystem : ASystem, IOnUpdate, IOnFixedUpdate, IOnLateUpdate
{
    private APlayer _player;
    public override void OnSetUp()
    {
        _player = gameStat.player;

        PlayerFieldOfView playerFieldOfView = new PlayerFieldOfView
        (
            gameStat.meshFilter, 
            new Mesh(), 
            gameStat.targetLayer, 
            gameStat.obstacleLayer, 
            gameStat.meshResolution, 
            gameStat.edgeResolveIterations, 
            gameStat.edgeDstThreshold
        );

        _player.OnSetUp(new Entity_HealthPoint(100, 100), playerFieldOfView, gameStat.radius, gameStat.angle);

        gameStat.onPlayerAttackEvent += OnAttack;
        gameStat.onPlayerReloadEvent += OnReload;
    }

    public void OnUpdate()
    {
        
    }

    public void OnLateUpdate()
    {
        _player.DrawView();
    }

    public void OnFixedUpdate()
    {
        //画面上のプレイヤー移動方向と入力方向が異なってみえる　45度回転することで解決
        float x = (gameStat.moveDirection.x) * Mathf.Cos(-45 * Mathf.Deg2Rad) - (gameStat.moveDirection.y) * Mathf.Sin(-45 * Mathf.Deg2Rad);
        float z = (gameStat.moveDirection.x) * Mathf.Sin(-45 * Mathf.Deg2Rad) + (gameStat.moveDirection.y) * Mathf.Cos(-45 * Mathf.Deg2Rad);
        Vector2 vector = new Vector3(x, z);

        _player.OnMove(vector, gameStat.cursorWorldPosition);
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
