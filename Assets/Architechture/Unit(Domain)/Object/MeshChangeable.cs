using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshChangeable : MonoBehaviour
{
    protected MeshRenderer _entityRenderer;
    protected MeshRenderer[] _entityChildrenMeshArray;
    [SerializeField]private bool _autoDisable = false;
    
    // Dirty Flagパターン用の状態管理
    private bool _isVisible = true;
    private bool _isDirty = false;
    private bool _isInitialized = false;
    
    void Start()
    {
        Initialize();
        
        if (_autoDisable)
        {
            SetVisibility(false);
        }
    }
    
    private void Initialize()
    {
        if (_isInitialized) return;
        _entityRenderer = GetComponent<MeshRenderer>();
        _entityChildrenMeshArray = GetComponentsInChildren<MeshRenderer>();
        _isInitialized = true;
    }

    public void SetVisibility(bool isVisible)
    {
        if (!_isInitialized) Initialize();
        
        // 状態が変更されていない場合は何もしない
        if (_isVisible == isVisible) return;
        
        _isVisible = isVisible;
        _isDirty = true;
        
        ApplyVisibilityChange();
    }
    
    private void ApplyVisibilityChange()
    {
        if (!_isDirty) return;
        
        if (_entityRenderer != null)
        {
            _entityRenderer.enabled = _isVisible;
        }
        
        if (_entityChildrenMeshArray != null)
        {
            foreach(MeshRenderer mesh in _entityChildrenMeshArray)
            {
                mesh.enabled = _isVisible;
            }
        }
        
        _isDirty = false;
    }
}
