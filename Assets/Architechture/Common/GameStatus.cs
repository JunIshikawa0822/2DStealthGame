using System;
using UnityEngine;
using TMPro;

[System.Serializable]
public class GameStatus
{
    [Header("PlayerActions")]
    public Action onPlayerAttackEvent;
    public Action onPlayerReloadEvent;

    [Header("Inputs")]
    public Vector2 moveDirection = Vector2.zero;
    public Vector2 cursorScreenPosition = Vector2.zero;
    public Vector3 cursorWorldPosition = Vector3.zero;
    //public bool onAttack = false;
    public GameObject cursorObject;

    [Header("PlayerGunsInfo")]
    public HandGun Pistol1;

    [HideInInspector]
    public IGun[] playerGunsArray = new IGun[2];
    public int selectingGunsArrayIndex = 0;

    [Header("PlayerInfo")]
    public APlayer player;

    [Header("Bullets")]
    public Transform bulletObjectPoolTrans;
    public Bullet_10mm bullet_10mm;
    public Bullet_5_56mm bullet_5_56mm;
    public Bullet_7_62mm bullet_7_62mm;

    [Header("TextUI")]
    public TextMeshProUGUI AmmoText;
}
