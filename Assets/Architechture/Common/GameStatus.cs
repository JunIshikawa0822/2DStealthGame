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
    public Action onSelectGunChange;

    #region 不使用
    public Action<int, ItemData> onEquipEvent;
    public Action<int> onUnEquipEvent;
    #endregion

    public Action<int, I_Data_Item> onPlayerEquipEvent;
    public Action<int, I_Data_Item> onPlayerUnEquipEvent;

    [Header("Inputs")]
    public Vector2 moveDirection = Vector2.zero;
    public Vector2 cursorScreenPosition = Vector2.zero;
    public Vector3 cursorWorldPosition = Vector3.zero;
    //public bool onAttack = false;
    public GameObject cursorObject;
    public UnityEngine.UI.Image cursorImage;

    [Header("PlayerInfo")]
    [HideInInspector]public Entity_HealthPoint playerHP;
    [HideInInspector]public AGun[] playerGunsArray  = new AGun[2];
    [HideInInspector]public int selectingGunsArrayIndex = 0;

    [Header("GunPrefabs")]
    //public Handgun handgunPrefab;
    public Handgun[] handgunPrefabs;
    public Shotgun[] shotgunPrefabs;

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

    #region 不使用
    public List<AInventory> inventoryList = new List<AInventory>();
    public GUI_Item gui_Item_Prefab;
    #endregion

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
#endregion

    [Header("Facade")]
    public Transform gunInstanceParent;

    public ItemFacade itemFacade;
    public GunFacade gunFacade;

    //public List<IGunFactory> gunFactoriesList;

}
