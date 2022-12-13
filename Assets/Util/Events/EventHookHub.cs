using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exo.Events
{
    public class EventHookHub : MonoBehaviour
    {
        public readonly HookableEvent hub = new HookableEvent("Hub");

        [SerializeField, Required]
        public List<EventHook> hooks;

        private void OnEnable()
        {
            foreach (EventHook hook in hooks)
            {
                hook.AddListener(hub.Invoke);
            }
        }

        private void OnDisable()
        {
            foreach (EventHook hook in hooks)
            {
                hook.RemoveListener(hub.Invoke);
            }
        }

    }
}
