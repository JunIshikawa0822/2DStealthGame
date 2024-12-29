using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using Microsoft.Unity.VisualStudio.Editor;

[System.Serializable]
public class GameStatus
{
    [Header("PlayerActions")]
    public Action onPlayerAttackEvent;
    public Action onPlayerReloadEvent;
    public Action onInventoryActiveEvent;

    public Action<int, ItemData> onEquipEvent;
    public Action<int> onUnEquipEvent;

    [Header("Inputs")]
    public Vector2 moveDirection = Vector2.zero;
    public Vector2 cursorScreenPosition = Vector2.zero;
    public Vector3 cursorWorldPosition = Vector3.zero;
    //public bool onAttack = false;
    public GameObject cursorObject;
    public UnityEngine.UI.Image cursorImage;

    [Header("PlayerGunsInfo")]
    //public Handgun Pistol1;
    [HideInInspector]public AGun[] playerGunsArray = new AGun[2];
    [HideInInspector]public int selectingGunsArrayIndex = 0;

    [Header("GunPrefabs")]
    //public Handgun handgunPrefab;
    public Handgun[] handgunPrefabs;
    public Shotgun[] shotgunPrefabs;

    [Header("PlayerInfo")]
    public PlayerController player;

    [Header("EnemiesInfo")]
    public Enemy_Bandit_Controller bandit;

    [Header("ItemData")]
    public ScriptableObject[] itemDataArray;
    
    public Food_Data[] foodDataArray;
    public Medicine_Data[] medicineDataArray;

    public Handgun_Data[] handgunDataArray;
    public Rifle_Data[] rifleDataArray;
    public Shotgun_Data[] shotgunDataArray;
    public SubMachinegun_Data[] subMachinegunDataArray;

    [Header("Bullets")]
    public Transform bulletObjectPoolTrans;
    public Bullet_10mm bullet_10mm;
    public Bullet_5_56mm bullet_5_56mm;
    public Bullet_7_62mm bullet_7_62mm;

    [Header("TextUI")]
    public TextMeshProUGUI AmmoText;

    [Header("PlayerFieldOfView")]
    public float radius;

    [Range(0, 360)]
    public float angle;
    public MeshFilter meshFilter;
    public LayerMask targetLayer;
    public LayerMask obstacleLayer;
    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    [Header("UI")]
    public LineRenderer shotLineRenderer;

    [Header("UGUI")]
    public Canvas canvas;
    //public PlayerEquipInventory equipInventory1;
    //public PlayerEquipInventory equipInventory2;

    public List<AInventory> inventoryList = new List<AInventory>();
    //public Inventory inventory1;
    //public Inventory inventory2;
    public GUI_Item gui_Item_Prefab;
    public Storage playerStorage;
    public Storage otherStorage = null;

    public GameObject inventoryPanel;
    public bool isInventoryPanelActive = false;

#region  即席
    [Header("Enemy")]
    public Transform enemyParent;
#endregion

    [Header("Facade")]
    public Transform gunInstanceParent;


    public ItemFacade itemFacade;
    public GunFacade gunFacade;
    

    //public List<IGunFactory> gunFactoriesList;

}
