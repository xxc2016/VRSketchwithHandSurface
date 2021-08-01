using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
public class ColorPickClick : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Button.ButtonClickedEvent Click;

    public Vector3 ClickPoint { get; set; }

    public void OnDrag(PointerEventData eventData)
    {
        var rect = transform as RectTransform;
        ClickPoint = rect.InverseTransformPoint(eventData.position);
        Click.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var rect = transform as RectTransform;
        ClickPoint = rect.InverseTransformPoint(eventData.position);
        Click.Invoke();
    }
}