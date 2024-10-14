using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSector : MonoBehaviour
{
    [Header("Scene assets")] [SerializeField]
    //謎のSceneLoader
    SceneLoader m_SceneLoader;
    //謎のScenePath
    [SerializeField] string m_ScenePath;

    //補正があるらしい
    [Tooltip("Offset to transform position")]
    public Vector3 m_CenterOffset;

    //ロードする半径
    [Tooltip("Minimum distance to load")] public float m_LoadRadius;

    //Activeな時に使用するMaterial
    [Header("Visualization")] [Tooltip("Material used when the sector's content is loaded.")] [SerializeField]
    Material m_ActiveMaterial;

    //inActivaな時に使用するMaterial
    [Tooltip("Material used when the sector's content is unloaded.")] [SerializeField]
    Material m_InactiveMaterial;

    //謎のMeshRenderer
    // Reference to the MeshRenderer for visualization
    MeshRenderer m_MeshRenderer;

    // Properties
    public bool IsLoaded { get; private set; } = false;
    public bool IsDirty { get; private set; } = false;

    void Awake()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();

        //シーン内に存在する最初のSceneLoaderを見つける
        m_SceneLoader = FindFirstObjectByType<SceneLoader>();

        if (m_SceneLoader == null)
        {
            Debug.LogError("[Sector]: SceneLoader not found in the scene.");
        }

        // Reset the dirty flag to start
        Clean();

        IsLoaded = false;

        Debug.Log($"IsLoaded : {IsLoaded} , IsDirty : {IsDirty}");
        
        if(m_ScenePath == null)return;
        Debug.Log("パス！" + m_ScenePath);
    }

    // Mark the sector as needing an update
    public void MarkDirty()
    {
        IsDirty = true;

        Debug.Log("Sector " + gameObject.name + " is marked dirty");
    }

    // Load sector content
    public void LoadContent()
    {
        // Implement content loading logic
        IsLoaded = true;

        // MaterialをActiveにする
        if (m_MeshRenderer != null)
            m_MeshRenderer.material = m_ActiveMaterial;

        Debug.Log($"{gameObject.name} Loading sector content...");
        
        //m_ScenePathがあるならm_ScenePathを加算してロード
        Debug.Log(!string.IsNullOrEmpty(m_ScenePath));

        if (!string.IsNullOrEmpty(m_ScenePath))
        {
            Debug.Log("ロード!!");
            m_SceneLoader.LoadSceneAdditivelyByPath(m_ScenePath);
            Debug.Log($"{m_ScenePath}ロードしたよ");
        }
    }

    // Unload sector content
    public void UnloadContent()
    {
        // Content unloading logic
        IsLoaded = false;

        if (m_MeshRenderer != null)
            m_MeshRenderer.material = m_InactiveMaterial;

        m_SceneLoader.UnloadSceneByPath(m_ScenePath);
        Debug.Log($"Unloading sector content...{m_ScenePath}");
    }

    // Check if the player is close enough to consider loading this sector
    public bool IsPlayerClose(Vector3 playerPosition)
    {
        return Vector3.Distance(playerPosition, transform.position + m_CenterOffset) <= m_LoadRadius;
    }

    

    // Reset the dirty flag after updating
    public void Clean()
    {
        IsDirty = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + m_CenterOffset, m_LoadRadius);
    }

    void OnDestroy()
    {
        m_SceneLoader.UnloadSceneImmediately(m_ScenePath);
    }
}
