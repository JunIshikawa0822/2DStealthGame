using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InGameManager : MonoBehaviour
{
    [SerializeField]
    GameStatus _gameStat;
    List<ASystem> _allSystemsList;
    List<IOnUpdate> _allUpdateSystemsList;
    List<IOnPreUpdate> _allPreUpdateSystemsList;
    List<IOnFixedUpdate> _allFixedUpdateSystemsList;

    List<IOnLateUpdate> _allLateUpdateSystemList;
    void Awake()
    {
        _allSystemsList = new List<ASystem>
        {
            //new ShotSystem(),
            new InputSystem(),
            new GunSystem(),
            new PlayerSystem(),
            new UISystem(),
            new EnemySystem()
        };

        _allUpdateSystemsList = new List<IOnUpdate>();
        _allPreUpdateSystemsList = new List<IOnPreUpdate>();
        _allFixedUpdateSystemsList = new List<IOnFixedUpdate>();
        _allLateUpdateSystemList = new List<IOnLateUpdate>();

        foreach (ASystem system in _allSystemsList)
        {
            system.OnGameStatusInit(_gameStat);

            if (system is IOnUpdate) _allUpdateSystemsList.Add(system as IOnUpdate);
            if (system is IOnPreUpdate) _allPreUpdateSystemsList.Add(system as IOnPreUpdate);
            if (system is IOnFixedUpdate) _allFixedUpdateSystemsList.Add(system as IOnFixedUpdate);
            if (system is IOnLateUpdate) _allLateUpdateSystemList.Add(system as IOnLateUpdate);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (ASystem system in _allSystemsList) system.OnSetUp();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (IOnPreUpdate system in _allPreUpdateSystemsList) system.OnPreUpdate();
        foreach (IOnUpdate system in _allUpdateSystemsList) system.OnUpdate();
    }

    void FixedUpdate()
    {
        foreach(IOnFixedUpdate system in _allFixedUpdateSystemsList) system.OnFixedUpdate();
    }

    void LateUpdate()
    {
        foreach(IOnLateUpdate system in _allLateUpdateSystemList) system.OnLateUpdate();
    }
}
