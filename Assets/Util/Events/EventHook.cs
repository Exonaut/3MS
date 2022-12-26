using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;

[System.Serializable]
public class EventHook<E> where E : UnityEngine.Object
{
    [SerializeField, InfoBox("Required", InfoMessageType.Error, "ShowRequired"), OnValueChanged("Clear"), SceneObjectsOnly] private MonoBehaviour target;
    [SerializeField, ValueDropdown("GetEventNames")] private string eventName = "";

    List<System.Reflection.EventInfo> GetEvents()
    {
        return target.GetType().GetEvents()
            .Where(e => e.GetCustomAttributes(typeof(Hookable), inherit: true).Any()
                && GetType().GetGenericArguments()[0].IsAssignableFrom(e.EventHandlerType.GetGenericArguments()[0]))
            .ToList();
    }

    IEnumerable GetEventNames()
    {
        var eventNames = new ValueDropdownList<string>();
        eventNames.Add("");

        GetEvents().ForEach((e) => eventNames.Add(e.Name));

        return eventNames;
    }

    void Clear()
    {
        eventName = "";
    }

    public void AddListener(Action<E> action)
    {
        var events = GetEvents();
        if (target == null || eventName == "" || events == null) return;

        var e = events.Find(x => x.Name == eventName);
        e.AddEventHandler(target, action);
    }

    public void RemoveListener(Action<E> action)
    {
        var events = GetEvents();
        if (target == null || eventName == "" || events == null) return;

        var e = events.Find(x => x.Name == eventName);
        e.RemoveEventHandler(target, action);
    }

    public static EventHook<E> operator +(EventHook<E> a, Action<E> action)
    {
        a.AddListener(action);
        return a;
    }

    public static EventHook<E> operator -(EventHook<E> a, Action<E> action)
    {
        a.RemoveListener(action);
        return a;
    }

    public bool ShowRequired()
    {
        if (target == null || eventName == "") return true;
        return false;
    }
}

public class Hookable : Attribute
{
    public Hookable()
    {

    }
}