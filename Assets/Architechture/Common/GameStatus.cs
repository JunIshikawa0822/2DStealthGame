using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameStatus
{
    public bool onClick = false;

    public Pistol1 Pistol1;

    public Bullet_10mm_Factories bullet_10mm_Factories;

    public Bullet_Factories bullet_Factories;
    public ObjectPool obj;
}
