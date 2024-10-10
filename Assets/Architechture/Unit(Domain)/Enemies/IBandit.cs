using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBandit
{
    protected enum BanditStatus
    {
        Usual, //普通
        Warn, //警戒
        Caution //注意
    }

    protected enum BanditAction
    {
        Standing,
        Walking,
        Running
    }

    protected enum BanditBattleAction
    {
        Idle,
        Hiding,
        Healing,
        Reloading,
        Attacking
    }
}
