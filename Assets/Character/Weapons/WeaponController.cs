using System.Collections;
using System.Collections.Generic;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField, AssetsOnly, AssetList(Path = "Abilities/Abilities")]
    public Ability weaponUseAbility;
    public string weaponName;
    public string weaponDescription;
    public Sprite weaponIcon;

    [SerializeField] private List<Transform> aimOrigin;
    [SerializeField] private List<Transform> weaponEffectOrigin;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Logger logger;

    [SerializeField] private bool canFire = true;
    [SerializeField] private bool hasFired = false;
    [SerializeField] private int shotCount = 0;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        canFire = true;
        hasFired = false;
        shotCount = 0;
    }

    public Hitable[] Fire(float holdTime, LayerMask abilityLayerMask, Transform playerCamera = null, Transform target = null)
    {
        if (holdTime == 0f)
        {
            hasFired = false;
            shotCount = 0;
        }

        var canhold = weaponUseAbility.canHold;
        if (canFire && ((canhold && holdTime >= weaponUseAbility.abilityCastTime) || (!canhold && !hasFired && holdTime >= weaponUseAbility.abilityCastTime)))
        {
            StartCoroutine(FireCooldown());
            hasFired = true;
            Fire(abilityLayerMask, playerCamera, target);
            print("fire");
        }
        return null;
    }

    private void Fire(LayerMask abilityLayerMask, Transform playerCamera = null, Transform target = null)
    {
        if (logger) logger.Log($"Fired {weaponUseAbility.abilityName}");
        var attackOrigin = (aimOrigin.Count > 0 || playerCamera == null) ? aimOrigin : new List<Transform>() { playerCamera };

        var ignores = new List<Collider>(GetComponentsInChildren<Collider>());

        var i = (shotCount + 1) % attackOrigin.Count;
        var q = (i + 1) % weaponEffectOrigin.Count;
        weaponUseAbility.UseAbility(ignores, attackOrigin[i], abilityLayerMask, weaponEffectOrigin[q], logger, target);

        shotCount++;
    }

    // Create coroutine wich enables firing after cooldown
    public IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(weaponUseAbility.abilityCooldown);
        canFire = true;
    }
}
