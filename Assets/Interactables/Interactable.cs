using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

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

        [Hookable] public event Action<Object> onInteract;
        [Hookable] public event Action<Interactable> onInteractInteractable;
        [Hookable] public event Action<Interactor> onInteractInteractor;
        public virtual void Interact(Interactor interactor)
        {
            if (canInteract && onInteract != null)
            {
                logger.Log("Interacted with by " + interactor.name, this);
                onInteract?.Invoke(this);
                onInteractInteractable?.Invoke(this);
                onInteractInteractor?.Invoke(interactor);
            }
        }
    }
}
