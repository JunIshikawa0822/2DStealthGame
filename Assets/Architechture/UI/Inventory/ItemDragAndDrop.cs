
using UnityEngine;
using UnityEngine.EventSystems;
using System;
    
public abstract class ItemDragAndDrop : MonoBehaviour
{
    public event Action OnPointerDownEvent;
    public event Action OnBeginDragEvent;
    public event Action OnEndDragEvent;
    public event Action OnDragEvent;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(OnPointerDownEvent == null)return;
        OnPointerDownEvent.Invoke();
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        if(OnBeginDragEvent == null)return;
        OnBeginDragEvent.Invoke();
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        if(OnEndDragEvent == null)return;
        OnEndDragEvent.Invoke();
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        if(OnDragEvent == null)return;
        OnDragEvent.Invoke();
    }
}
