﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIEventTriggerListener : UnityEngine.EventSystems.EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;

        static public UIEventTriggerListener Get(GameObject go)
        {
            UIEventTriggerListener listener = go.GetComponent<UIEventTriggerListener>();
            if (listener == null) listener = go.AddComponent<UIEventTriggerListener>();
            return listener;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null) onClick(gameObject);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null) onDown(gameObject);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null) onEnter(gameObject);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null) onExit(gameObject);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null) onUp(gameObject);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null) onSelect(gameObject);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (onUpdateSelect != null) onUpdateSelect(gameObject);
        }
    }
}
