using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshChangable : MonoBehaviour
{
    protected MeshRenderer _entityRenderer;
    protected MeshRenderer[] _entityChildrenMeshsArray;
    void Awake()
    {
        _entityRenderer = GetComponent<MeshRenderer>();
        _entityChildrenMeshsArray= GetComponentsInChildren<MeshRenderer>(); 
    }

    void Start()
    {
        
        Debug.Log(_entityRenderer);
        EntityMeshDisable();
    }

    public void EntityMeshDisable()
    {
        //Debug.Log("消えた");
        _entityRenderer.enabled = false;

        foreach(MeshRenderer mesh in _entityChildrenMeshsArray)
        {
            mesh.enabled = false;
        }
    }

    public void EntityMeshAble()
    {
        // Debug.Log("ついた");
        Debug.Log(_entityRenderer == null);
        _entityRenderer.enabled = true;

        foreach(MeshRenderer mesh in _entityChildrenMeshsArray)
        {
            mesh.enabled = true;
        }
    }
}
