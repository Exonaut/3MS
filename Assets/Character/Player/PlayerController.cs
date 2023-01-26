using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Interaction;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    PlayerAbilityController abilityController;
    PlayerMovementController movementController;
    PlayerInteractionController interactionController;
    Hitable hitable;
    CharacterController characterController;

    [FoldoutGroup("Dependencies", true)][SerializeField, Required] Logger logger;
    [FoldoutGroup("Dependencies", true)][SerializeField, Required] Object endGameScene;

    [FoldoutGroup("Info", true)][SerializeField, ReadOnly] Vector3 startPosition;
    [FoldoutGroup("Info", true)][SerializeField, ReadOnly] Quaternion startRotation;

    bool initialized = false;

    private void OnEnable()
    {
        if (!logger) logger = Logger.GetDefaultLogger(this);

        abilityController = GetComponent<PlayerAbilityController>();
        movementController = GetComponent<PlayerMovementController>();
        interactionController = GetComponent<PlayerInteractionController>();
        hitable = GetComponent<Hitable>();
        characterController = GetComponent<CharacterController>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        initialized = true;

        hitable.onDie += LoadGameLostScene;
    }

    private void LoadGameLostScene(Hitable obj)
    {
        GameGlobals.gameWon = false;
        SceneManager.LoadScene(endGameScene.name);
    }

    public void Restart()
    {
        if (!initialized) return;

        logger?.Log("Restarting player");
        hitable.Restart();
        characterController.enabled = false;
        transform.SetPositionAndRotation(startPosition, startRotation);
        characterController.enabled = true;
    }
}
