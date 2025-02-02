using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class GameStatus
{
    [Header("Environment")]
    [HideInInspector] public bool isCombatAllow = true;
    [HideInInspector] public bool isInventoryAllow = true;
    [HideInInspector] public bool isPlayerMovementAllow = true;
    [HideInInspector] public SceneLoader[] sceneLoader;
    [SerializeField] public Camera camera;

    [Header("PlayerActions")]
    public Action onPlayerAttackStartEvent;
    public Action onPlayerAttackingEvent;
    public Action onPlayerAttackEndEvent;
    public Action onPlayerReloadEvent;
    public Action onInventoryActiveEvent;
    public Action onSelectGunChange;

    public Action<int, I_Data_Item> onPlayerEquipEvent;
    public Action<int, I_Data_Item> onPlayerUnEquipEvent;

    [Header("Inputs")]
    [HideInInspector]public Vector2 moveDirection = Vector2.zero;
    [HideInInspector]public Vector2 cursorScreenPosition = Vector2.zero;
    [HideInInspector]public Vector3 cursorWorldPosition = Vector3.zero;
    [HideInInspector]public Vector2 cursorAlignShotPosition = Vector2.zero;
    //public bool onAttack = false;
    public Transform cursorObject;
    public Image cursorImage;

    [SerializeField] public LayerMask mouseLayHitlayer = 1 << 6;

    [Header("PlayerInfo")]
    [HideInInspector]public Entity_HealthPoint playerHP;
    [HideInInspector]public AGun[] playerGunsArray  = new AGun[2];
    [HideInInspector]public int selectingGunsArrayIndex = 0;

    [Header("GunPrefabs")]
    //public Handgun handgunPrefab;
    public Handgun[] handgunPrefabs;
    public Shotgun[] shotgunPrefabs;
    public Rifle[] riflePrefabs;
    
    [Header("Player")]
    public PlayerController player;

    [Header("EnemiesInfo")]
    public Enemy_Bandit_Controller bandit;

    [Header("ItemData")]

    public Data_Fixed_Food[] data_Fixed_Food_Array;
    public Data_Fixed_Medicine[] data_Fixed_Medicine_Array;
    public Data_Fixed_Handgun[] data_Fixed_Handgun_Array;
    public Data_Fixed_Rifle[] data_Fixed_Rifle_Array;
    public Data_Fixed_Shotgun[] data_Fixed_Shotgun_Array;

    [Header("Bullets")]
    public Transform bulletObjectPoolTrans;
    public Bullet_10mm bullet_10mm;
    public Bullet_5_56mm bullet_5_56mm;
    public Bullet_7_62mm bullet_7_62mm;

    [Header("UI")]
    public LineRenderer shotLineRenderer;
    public TextMeshProUGUI ammoText;
    public Slider playerHPSlider;
    public Transform item_GUI_PoolTrans;

    [Header("UGUI")]
    public Canvas canvas;
    //public PlayerEquipInventory equipInventory1;
    //public PlayerEquipInventory equipInventory2; 
    //public Inventory inventory1;
    //public Inventory inventory2;

    public List<A_Inventory> inventories = new List<A_Inventory>();
    public Item_GUI item_GUI_Prefab;

    //[HideInInspector]public Storage playerStorage;
    //[HideInInspector]public Storage otherStorage = null;
    [HideInInspector]public IStorage playerStorage = null;
    [HideInInspector]public IStorage otherStorage = null;
    [HideInInspector]public IStorage[] weaponStorages = new WeaponStorage[2];

    public GameObject inventoryPanel;
    public bool isInventoryPanelActive = false;

#region  即席
    [Header("Enemy")]
    public Transform enemyParent;
    public List<AEnemy> enemyObjects;
#endregion

    [Header("Facade")]
    public Transform gunInstanceParent;

    public ItemFacade itemFacade;
    public GunFacade gunFacade;

    //public List<IGunFactory> gunFactoriesList;
    [Header("MortonSpace")]
    [SerializeField] public float cellWidth;
    [SerializeField] public float cellHeight;
    [SerializeField] public float cellDepth;
    [SerializeField] public int dimensionLevel;
    [SerializeField] public Transform mortonSpaceBaseTrans;
    [SerializeField] public Transform targetParent;

    [SerializeField] public Transform testObject;
}
