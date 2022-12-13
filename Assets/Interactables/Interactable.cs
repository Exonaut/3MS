using System.Collections;
using System.Collections.Generic;
using Exo.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour
    {
        [FoldoutGroup("Dependencies", expanded: true)]
        [FoldoutGroup("Dependencies")][SerializeField, Required] private Logger logger;

        [FoldoutGroup("Settings", expanded: true)]
        [FoldoutGroup("Settings")][ShowInInspector] public string interactableName;
        [FoldoutGroup("Settings")][ShowInInspector] public bool canInteract = true;

        private void OnEnable()
        {
            if (logger == null)
            {
                logger = Logger.GetDefaultLogger(this);
            }
        }

        public readonly HookableEvent onInteract = new HookableEvent("Interact");
        public virtual void Interact(Interactor interactor)
        {
            if (canInteract && onInteract != null)
            {
                logger.Log("Interacted with by " + interactor.name, this);
                onInteract.Invoke(this);
            }
        }
    }
}
