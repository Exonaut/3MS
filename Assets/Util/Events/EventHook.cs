using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Exo.Events
{
    [System.Serializable]
    public class EventHook
    {
        [SerializeField, InfoBox("Required", InfoMessageType.Error, "ShowRequired"), SceneObjectsOnly] private MonoBehaviour target;
        [SerializeField, ValueDropdown("GetEventNames"), OnValueChanged("SetHook")] private string eventName = "";

        protected HookableEvent eventAction 
        { 
        get 
        {
            GetEventNames();
            return SetHook();
        }}

        List<HookableEvent> hookableEvents;

        /// <summary>
        /// Returns a list of all the events that can be hooked to.
        /// </summary>
        /// <returns></returns>
        IEnumerable GetEventNames()
        {
            // Setup name list
            var eventNames = new ValueDropdownList<string>();
            eventNames.Add("");

            // Setup event list
            if (hookableEvents == null) hookableEvents = new List<HookableEvent>();
            hookableEvents.Clear();

            if (target == null) return null;

            var type = target.GetType();

            // Add all public property events
            var properties = target.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(HookableEvent))
                {
                    var _event = property.GetValue(target) as HookableEvent;
                    hookableEvents.Add(_event);
                    eventNames.Add(_event.eventName);
                }
            }

            // Add all public field events
            var fields = target.GetType().GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(HookableEvent))
                {
                    var _event = field.GetValue(target) as HookableEvent;
                    hookableEvents.Add(_event);
                    eventNames.Add(_event.eventName);
                }
            }

            eventNames.OrderBy(x => x); // Sort alphabetically
            if (hookableEvents.Find(x => x.eventName == eventName) == null) eventName = "";

            return eventNames;
        }

        HookableEvent SetHook()
        {
            if (target == null || eventName == "")
            {
                return null;
            }
            return hookableEvents.Find(x => x.eventName == eventName);
        }

        [Button("Invoke"), ShowIf("eventAction", null)]
        void Invoke()
        {
            if (eventAction == null) return;
            eventAction.Invoke(target);
        }

        public void AddListener(Action<MonoBehaviour> b)
        {
            eventAction.AddListener(b);
        }

        public void RemoveListener(Action<MonoBehaviour> b)
        {
            eventAction.RemoveListener(b);
        }

        public static EventHook operator +(EventHook a, Action<MonoBehaviour> b)
        {
            a.eventAction.AddListener(b);
            return a;
        }

        public static EventHook operator -(EventHook a, Action<MonoBehaviour> b)
        {
            a.eventAction.RemoveListener(b);
            return a;
        }

        public bool ShowRequired()
        {
            if (target == null || eventAction == null) return true;
            return false;
        }

    }
}
