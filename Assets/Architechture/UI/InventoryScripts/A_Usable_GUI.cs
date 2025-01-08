using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public abstract class A_Interactive_GUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected RectTransform _rectTransform;
    public event Action<A_Interactive_GUI> onPointerDownEvent;
    public event Action<A_Interactive_GUI> onPointerEnterEvent;
    public event Action<A_Interactive_GUI> OnPointerExitEvent;
    public event Action<A_Interactive_GUI> onBeginDragEvent;
    public event Action<A_Interactive_GUI> onEndDragEvent;
    public event Action<A_Interactive_GUI> onDragEvent;
    public event Action<A_Interactive_GUI> onDestroyEvent;

    public virtual void OnSetUp()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public virtual void SetPosition(Vector3 position)
    {
        _rectTransform.position = position;
    }

    public virtual void SetAnchorPosition(Vector3 position)
    {
        _rectTransform.anchoredPosition = position;
    }

    public virtual void SetPivot(Vector2 pivot)
    {
        _rectTransform.pivot = pivot;
    }

    public virtual void SetRotation(Quaternion quaternion)
    {
        _rectTransform.rotation = quaternion;
    }

    public virtual void OnDestroy()
    {
        onDestroyEvent?.Invoke(this);
        Destroy(this.gameObject);
    }

    public virtual void OnPointerEnter(PointerEventData pointerEventData)
    {
        //説明文表示もしたい
        onPointerEnterEvent?.Invoke(this);
    }

    public virtual void OnPointerExit(PointerEventData pointerEventData)
    {
        OnPointerExitEvent?.Invoke(this);
    }

    public virtual void OnPointerDown(PointerEventData pointerEventData)
    {
        onPointerDownEvent?.Invoke(this);
    }

    public virtual void OnBeginDrag(PointerEventData pointerEventData)
    {
        onBeginDragEvent?.Invoke(this);
    }

    public virtual void OnEndDrag(PointerEventData pointerEventData)
    {
        onEndDragEvent?.Invoke(this);
    }

    public virtual void OnDrag(PointerEventData pointerEventData)
    {
        onDragEvent?.Invoke(this);
    }
}
