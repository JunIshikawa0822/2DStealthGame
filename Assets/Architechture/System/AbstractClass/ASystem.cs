using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASystem
{
    protected GameStatus gameStat;
    public void OnGameStatusInit(GameStatus gameStat)
    {
        this.gameStat = gameStat;
    }
    public abstract void OnSetUp();
}
