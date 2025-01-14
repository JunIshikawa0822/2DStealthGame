
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager :MonoBehaviour
{
    public InGameManager gameManager;
    public SceneLoader[] sceneLoaders;

    [SerializeField] bool _isInventoryAllow = false;
    [SerializeField] bool _isEnemySpawnAllow = false;

    [Header("Environment")]
    [SerializeField] bool _isCombatAllow = false;
    [SerializeField] bool _isUIActiveAllow = false;

    void Awake()
    {
        if(gameManager == null) return;

        List<ASystem> systems = new List<ASystem>();

        systems.Add(new GunSystem());
        systems.Add(new ItemSystem());
        systems.Add(new PlayerSystem());
        systems.Add(new SceneManageSystem());

        if(_isUIActiveAllow)systems.Add(new UISystem());
        if(_isInventoryAllow) systems.Add(new TetrisInventorySystem());
        if(_isEnemySpawnAllow) systems.Add(new EnemySystem());

        systems.Add(new InputSystem());
        gameManager.SetUp(systems, sceneLoaders, _isCombatAllow, _isInventoryAllow);
    }
}
