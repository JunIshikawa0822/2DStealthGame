using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingLoader : MonoBehaviour
{
    [SerializeField]
    Sector[] sectors;

    [SerializeField]
    Sector testSector;

    void Start()
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        
    }
}
