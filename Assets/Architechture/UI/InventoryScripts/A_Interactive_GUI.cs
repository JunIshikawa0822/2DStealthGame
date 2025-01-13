using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public abstract class A_Interactive_GUI : APooledObject, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected RectTransform _rectTransform;

    public RectTransform RectTransform{get => _rectTransform;}

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

    public virtual void SetScale(Vector3 scale)
    {
        _rectTransform.localScale = scale;
    }

    public virtual void SetRotation(Quaternion quaternion)
    {
        _rectTransform.rotation = quaternion;
    }

    public virtual void SetParent(Transform transform)
    {
        _rectTransform.SetParent(transform);
    }

    public virtual void OnDestroy()
    {
        //onDestroyEvent?.Invoke(this);
        Destroy(this.gameObject);
    }

    public virtual void OnPointerEnter(PointerEventData pointerEventData)
    {
        //説明文表示もしたい
        //onPointerEnterEvent?.Invoke(this);
    }

    public virtual void OnPointerExit(PointerEventData pointerEventData)
    {
        //OnPointerExitEvent?.Invoke(this);
    }

    public virtual void OnPointerDown(PointerEventData pointerEventData)
    {
        //onPointerDownEvent?.Invoke(this);
    }

    public virtual void OnPointerUp(PointerEventData pointerEventData)
    {
        //onPointerUpEvent?.Invoke(this);
    }

    public virtual void OnBeginDrag(PointerEventData pointerEventData)
    {
        //onBeginDragEvent?.Invoke(this);
    }

    public virtual void OnEndDrag(PointerEventData pointerEventData)
    {
        //onEndDragEvent?.Invoke(this);
    }

    public virtual void OnDrag(PointerEventData pointerEventData)
    {
        //onDragEvent?.Invoke(this);
    }
}
