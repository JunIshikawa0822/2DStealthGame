using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshChangable : MonoBehaviour
{
    protected MeshRenderer _entityRenderer;
    protected MeshRenderer[] _entityChildrenMeshsArray;

    void Start()
    {
        _entityRenderer = GetComponent<MeshRenderer>();
        _entityChildrenMeshsArray= GetComponentsInChildren<MeshRenderer>(); 
        EntityMeshDisable();
    }

    public void EntityMeshDisable()
    {
        // Debug.Log("消えた");
        _entityRenderer.enabled = false;

        foreach(MeshRenderer mesh in _entityChildrenMeshsArray)
        {
            mesh.enabled = false;
        }
    }

    public void EntityMeshAble()
    {
        // Debug.Log("ついた");
        _entityRenderer.enabled = true;

        foreach(MeshRenderer mesh in _entityChildrenMeshsArray)
        {
            mesh.enabled = true;
        }
    }
}
