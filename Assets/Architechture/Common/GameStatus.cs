using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameStatus
{
    public Vector2 moveDirection = Vector2.zero;
    public bool onAttack = false;

    public Pistol1 Pistol1;

    public Bullet_10mm_Factories bullet_10mm_Factories;
    public ObjectPool objectPool;

    public PlayerController player;

}
