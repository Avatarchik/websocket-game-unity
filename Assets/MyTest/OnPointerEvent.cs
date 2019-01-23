using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class OnPointerEvent : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler, IPointerClickHandler, IPointerEnterHandler
{
    public delegate void OnPointEventHandler(PointerEventData eventData);

    public UnityEvent onPress;
    public event OnPointEventHandler onPressEvent;

    public UnityEvent onUp;
    public event OnPointEventHandler onUpEvent;

    public UnityEvent onDown;
    public event OnPointEventHandler onDownEvent;

    public UnityEvent onExit;
    public event OnPointEventHandler onExitEvent;

    public UnityEvent onClick;
    public event OnPointEventHandler onClickEvent;

    public UnityEvent onEnter;
    public event OnPointEventHandler onEnterEvent;

    bool isMouseDown = false;

    public void OnPointerPress(PointerEventData eventData)
    {
        onPress.Invoke();
        if (onPressEvent != null)
            onPressEvent(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onUp.Invoke();
        if (onUpEvent != null)
            onUpEvent(eventData);

        isMouseDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onDown.Invoke();
        if (onDownEvent != null)
            onDownEvent(eventData);

        isMouseDown = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit.Invoke();
        if (onExitEvent != null)
            onExitEvent(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
        if (onClickEvent != null)
            onClickEvent(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter.Invoke();
        if (onEnterEvent != null)
            onEnterEvent(eventData);
    }

    void Update()
    {
        if (isMouseDown)
        {
            OnPointerPress(null);
        }
    }
}
