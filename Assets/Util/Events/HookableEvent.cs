using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exo.Events
{
    [System.Serializable]
    public class HookableEvent
    {
        public string eventName { get; private set; }
        public event Action<MonoBehaviour> eventAction;

        public HookableEvent(string eventName)
        {
            this.eventName = eventName;
        }

        public void Invoke(MonoBehaviour sender)
        {
            eventAction?.Invoke(sender);
        }

        public void AddListener(Action<MonoBehaviour> listener)
        {
            eventAction += listener;
        }

        public void RemoveListener(Action<MonoBehaviour> listener)
        {
            eventAction -= listener;
        }

        public static HookableEvent operator +(HookableEvent hookableEvent, Action<MonoBehaviour> action)
        {
            hookableEvent.eventAction += action;
            return hookableEvent;
        }

        public static HookableEvent operator -(HookableEvent hookableEvent, Action<MonoBehaviour> action)
        {
            hookableEvent.eventAction -= action;
            return hookableEvent;
        }
    }
}
