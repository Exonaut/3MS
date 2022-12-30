using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayerAbilityController : MonoBehaviour
{
    [FoldoutGroup("Dependencies")][SerializeField, Required] Transform playerCamera;
    [FoldoutGroup("Dependencies")][SerializeField, Required] WeaponController primaryWeapon;
    [FoldoutGroup("Dependencies")][SerializeField, Required] Logger logger;

    [FoldoutGroup("Masks")][SerializeField] LayerMask abilityLayerMask;

    InputHandler inputHandler;

    bool isPrimaryWeaponPressed = false;
    float primaryWeaponHoldingTime = 0f;

    void Start()
    {
        if (!inputHandler) inputHandler = FindObjectOfType<InputHandler>();
        if (!primaryWeapon) logger.LogWarning("Primary weapon not set", this);
        if (!logger) logger = Logger.GetDefaultLogger(this);

        if (inputHandler)
        {
            inputHandler.onPrimaryStart += holdAbilityStart;
            inputHandler.onPrimaryStop += holdAbilityStop;
        }
    }

    void holdAbilityStart(Object caller)
    {
        isPrimaryWeaponPressed = true;
        logger.Log($"Used {primaryWeapon.weaponName}");
    }

    void holdAbilityStop(Object caller)
    {
        isPrimaryWeaponPressed = false;
        primaryWeaponHoldingTime = 0f;
        logger.Log($"Stopped using {primaryWeapon.weaponName}");
    }

    private void Update()
    {
        if (isPrimaryWeaponPressed) // Primary Weapon hold
        {
            if (!primaryWeapon)
            {
                logger.LogWarning("Primary weapon not set", this);
                return;
            }

            primaryWeapon.Fire(primaryWeaponHoldingTime, abilityLayerMask, playerCamera);
            primaryWeaponHoldingTime += Time.deltaTime;
        }
    }
}
