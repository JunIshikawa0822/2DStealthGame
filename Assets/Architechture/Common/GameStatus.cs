using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameStatus
{
    public Action onPlayerAttackEvent;
    public Vector2 moveDirection = Vector2.zero;
    public Vector2 cursorScreenPosition = Vector2.zero;
    public Vector3 cursorWorldPosition = Vector3.zero;
    public bool onAttack = false;
    public GameObject cursorObject;
    public IGun Pistol1;
    public IBulletFactories bullet_Factories;
    public IObjectPool<ABullet> bulletObjectPool;
    public APlayer player;

}
