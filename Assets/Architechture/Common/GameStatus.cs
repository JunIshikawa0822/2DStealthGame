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

    public GameObject gameObject;

    public Pistol1 Pistol1;
    public Bullet_10mm_Factories bullet_10mm_Factories;

    public ObjectPool objectPool;
    public PlayerController player;

}
