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
    public Pistol1 Pistol1;
    public Bullet_Factories bullet_Factories;
    public IObjectPool<ABullet> objectPool_10mm;
    public PlayerController player;

}
