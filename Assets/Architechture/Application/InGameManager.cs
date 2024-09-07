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
    void Awake()
    {
        _allSystemsList = new List<ASystem>
        {
            //new ShotSystem(),
            new InputSystem(),
            new PlayerSystem()
        };

        _allUpdateSystemsList = new List<IOnUpdate>();
        _allPreUpdateSystemsList = new List<IOnPreUpdate>();
        _allFixedUpdateSystemsList = new List<IOnFixedUpdate>();

        foreach (ASystem system in _allSystemsList)
        {
            system.OnGameStatusInit(_gameStat);

            if (system is IOnUpdate) _allUpdateSystemsList.Add(system as IOnUpdate);
            if (system is IOnPreUpdate) _allPreUpdateSystemsList.Add(system as IOnPreUpdate);
            if (system is IOnFixedUpdate) _allFixedUpdateSystemsList.Add(system as IOnFixedUpdate);
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
        
    }
}
