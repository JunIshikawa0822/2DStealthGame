using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingLoader : MonoBehaviour
{
    public FloorSector[] m_Sectors;
    public PlayerController m_Player;
    private bool isPlayerInThis;
    private uint storiesOfBuilding;
    private uint playerFloor;

    [SerializeField]
    private Collider buildingCollider;

    private void Start()
    {
        storiesOfBuilding = 3;
        playerFloor = 0;
        isPlayerInThis = false;
    }

    private void Update()
    {
        if (m_Player == null)return;
        if(isPlayerInThis == false)return;

        foreach (FloorSector sector in m_Sectors)
        {
            bool isPlayerClose = sector.IsPlayerClose(m_Player.transform.position);

            // Check if the sector's state needs to change
            if (isPlayerClose != sector.IsLoaded)
            {
                //isDartyをオンにする
                sector.MarkDirty();
            }

            if (sector.IsDirty)
            {
                if (isPlayerClose)
                {
                    sector.LoadContent();
                }
                else
                {
                    sector.UnloadContent();
                }

                // Reset the dirty flag
                sector.Clean();
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        isPlayerInThis = true;
    }

    void OnTriggerExit(Collider collider)
    {
        isPlayerInThis = false;
    }
}
