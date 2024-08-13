using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InGameManager : MonoBehaviour
{
    [SerializeField]
    GameStatus gameStat;
    List<ASystem> allSystemsList;
    List<IOnUpdate> allUpdateSystemsList;
    List<IOnPreUpdate> allPreUpdateSystemsList;
    void Awake()
    {
        allSystemsList = new List<ASystem>
        {
            new ShotSystem(),
            new InputSystem()
        };

        allUpdateSystemsList = new List<IOnUpdate>();
        allPreUpdateSystemsList = new List<IOnPreUpdate>();

        foreach (ASystem system in allSystemsList)
        {
            system.OnGameStatusInit(gameStat);

            if (system is IOnUpdate) allUpdateSystemsList.Add(system as IOnUpdate);
            if (system is IOnPreUpdate) allPreUpdateSystemsList.Add(system as IOnPreUpdate);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (ASystem system in allSystemsList) system.OnSetUp();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (IOnPreUpdate system in allPreUpdateSystemsList) system.OnPreUpdate();
        foreach (IOnUpdate system in allUpdateSystemsList) system.OnUpdate();
    }

    void FixedUpdate()
    {

    }

    void LateUpdate()
    {
        
    }
}
