
using UnityEngine;
using UnityEngine.EventSystems;
using System;
    
public abstract class AItemDragAndDrop<T> : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public event Action<T> onPointerDownEvent;
    public event Action<T> onBeginDragEvent;
    public event Action<T> onEndDragEvent;
    public event Action<T> onDragEvent;

    protected T Tobject;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(onPointerDownEvent == null)return;
        onPointerDownEvent.Invoke(Tobject);
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        if(onBeginDragEvent == null)return;
        onBeginDragEvent.Invoke(Tobject);
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        if(onEndDragEvent == null)return;
        onEndDragEvent.Invoke(Tobject);
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        if(onDragEvent == null)return;
        onDragEvent.Invoke(Tobject);
    }
}
