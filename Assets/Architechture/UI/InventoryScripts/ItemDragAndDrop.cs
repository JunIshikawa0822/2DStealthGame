
using UnityEngine;
using UnityEngine.EventSystems;
using System;
    
public abstract class ItemDragAndDrop<T> : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public event Action<T> onPointerDownEvent;
    public event Action<T> onBeginDragEvent;
    public event Action<T> onEndDragEvent;
    public event Action<T> onDragEvent;

    protected T Tobject;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(onPointerDownEvent == null)return;
        //Debug.Log("OnPointerDown");
        onPointerDownEvent.Invoke(Tobject);
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        if(onBeginDragEvent == null)return;
        //Debug.Log("OnBeginDrag");
        onBeginDragEvent.Invoke(Tobject);
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        if(onEndDragEvent == null)return;
        //Debug.Log("OnEndDrag");
        onEndDragEvent.Invoke(Tobject);
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        if(onDragEvent == null)return;
        //Debug.Log("OnDrag");
        onDragEvent.Invoke(Tobject);
    }
}
