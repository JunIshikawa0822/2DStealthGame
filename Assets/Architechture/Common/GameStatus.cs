using System;
using UnityEngine;
using TMPro;

[System.Serializable]
public class GameStatus
{
    public Action onPlayerAttackEvent;
    public Action onPlayerReloadEvent;
    public Vector2 moveDirection = Vector2.zero;
    public Vector2 cursorScreenPosition = Vector2.zero;
    public Vector3 cursorWorldPosition = Vector3.zero;
    //public bool onAttack = false;
    public GameObject cursorObject;
    public HandGun Pistol1;
    public IGun[] playerGunsArray = new IGun[2];
    public int selectingGunsArrayIndex = 0;
    public APlayer player;
    public Transform bulletObjectPoolTrans;
    public Bullet_10mm bullet_10mm;
    public Bullet_5_56mm bullet_5_56mm;
    public Bullet_7_62mm bullet_7_62mm;

    [SerializeField]
    public TextMeshProUGUI AmmoText;
}
