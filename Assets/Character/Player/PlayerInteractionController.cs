using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Interaction
{
    public class PlayerInteractionController : Interactor
    {
        [FoldoutGroup("Dependencies", expanded: true)]
        [FoldoutGroup("Dependencies")][SerializeField, SceneObjectsOnly, Required] Camera playerCamera;

        [FoldoutGroup("Masks", expanded: true)]
        [FoldoutGroup("Masks")] public LayerMask interactionMask;

        [FoldoutGroup("Interaction Settings", expanded: true)]
        [FoldoutGroup("Interaction Settings")][Range(0f, 5f)] public float interactionDistance = 1.0f;

        InputHandler inputHandler;

        private void OnEnable()
        {
            // Find dependencies
            inputHandler = FindObjectOfType<InputHandler>();

            // Subscribe to input events
            if (inputHandler)
            {
                inputHandler.interactEvent += OnInteract;
            }

            if (!logger) logger = Logger.GetDefaultLogger(this);
        }

        private void OnDisable()
        {
            // Unsubscribe from input events
            if (inputHandler)
            {
                inputHandler.interactEvent -= OnInteract;
            }
        }

        void OnInteract()
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance, interactionMask))
            {
                Interactable interactable = hit.collider.GetComponentInChildren<Interactable>();
                if (interactable)
                {
                    logger.Log("Interacting with " + interactable.interactableName, this);
                    interactable.Interact(this);
                }
            }
        }

    }
}
