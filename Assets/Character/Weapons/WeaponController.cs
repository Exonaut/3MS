using System.Collections;
using System.Collections.Generic;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [FoldoutGroup("Dependencies", expanded: true)]
    [FoldoutGroup("Dependencies")][SerializeField, Required] protected Logger logger;
    [FoldoutGroup("Dependencies")][SerializeField] private List<Transform> aimOrigin;
    [FoldoutGroup("Dependencies")][SerializeField] private List<Transform> weaponEffectOrigin;
    [FoldoutGroup("Dependencies")][SerializeField] private LineRenderer lineRenderer;
    [FoldoutGroup("Dependencies")][SerializeField] private AudioSource audioSource;

    [FoldoutGroup("Properties", expanded: true)]
    [FoldoutGroup("Properties")][SerializeField, AssetsOnly] public Ability weaponUseAbility;
    [FoldoutGroup("Properties")] public string weaponName;
    [FoldoutGroup("Properties")] public string weaponDescription;
    [FoldoutGroup("Properties")] public Sprite weaponIcon;
    [FoldoutGroup("Properties")] public AudioClip weaponEmptySound;
    [FoldoutGroup("Properties")] public bool infiniteAmmo = false;
    [FoldoutGroup("Properties")][HideIf("infiniteAmmo"), Min(0)] public int maxAmmo = 16;
    [FoldoutGroup("Properties")][HideIf("infiniteAmmo"), PropertyRange(0, "maxAmmo")] public int currentAmmo = 16;

    private bool canFire = true;
    private bool hasFired = false;
    private int shotCount = 0;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        if (!logger) logger = Logger.GetDefaultLogger(this);

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
            if (!infiniteAmmo && currentAmmo <= 0)
            {
                if (weaponEmptySound && audioSource) audioSource.PlayOneShot(weaponEmptySound);
                return null;
            }
            Fire(abilityLayerMask, playerCamera, target);
            if (!infiniteAmmo) currentAmmo--;
        }
        return null;
    }

    private void Fire(LayerMask abilityLayerMask, Transform playerCamera = null, Transform target = null)
    {
        logger?.Log($"Fired {weaponUseAbility.abilityName}");
        var attackOrigin = (aimOrigin.Count > 0 || playerCamera == null) ? aimOrigin : new List<Transform>() { playerCamera };

        var ignores = new List<Collider>(GetComponentsInChildren<Collider>());

        var i = (shotCount + 1) % attackOrigin.Count;
        var q = (i + 1) % weaponEffectOrigin.Count;
        weaponUseAbility.UseAbility(ignores, attackOrigin[i], abilityLayerMask, weaponEffectOrigin[q], logger, target, audioSource);

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
