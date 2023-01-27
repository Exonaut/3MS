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
    [FoldoutGroup("Dependencies")][SerializeField] private AudioSource audioSource;
    [FoldoutGroup("Dependencies")][SerializeField] private Animator animator;

    [FoldoutGroup("Properties", expanded: true)]
    [FoldoutGroup("Properties")] public string weaponName;
    [FoldoutGroup("Properties")] public string weaponDescription;
    [FoldoutGroup("Properties")] public Sprite weaponIcon;
    [FoldoutGroup("Properties")] public bool infiniteAmmo = false;
    [FoldoutGroup("Properties")][HideIf("infiniteAmmo"), Min(0)] public int maxAmmo = 16;
    [FoldoutGroup("Properties")][HideIf("infiniteAmmo"), PropertyRange(0, "maxAmmo")] public int currentAmmo = 16;
    [FoldoutGroup("Properties")][SerializeField, AssetsOnly] public Ability weaponUseAbility;

    [FoldoutGroup("Sounds", expanded: true)]
    [FoldoutGroup("Sounds")] public AudioClip weaponEmptySound;

    [FoldoutGroup("Animations", expanded: true)]
    [FoldoutGroup("Animations")] public AnimationClip shootAnimation;

    public bool HasFullAmmo => currentAmmo == maxAmmo;

    private bool canFire = true;
    private bool hasFired = false;
    private int shotCount = 0;

    private void Start()
    {
        animator = animator == null ? GetComponent<Animator>() : animator;
        audioSource = audioSource == null ? GetComponent<AudioSource>() : audioSource;
    }

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
        if (!infiniteAmmo) currentAmmo--;

        if (animator) animator.Play("Shoot");
    }

    public void RestoreAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
    }

    public IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(weaponUseAbility.abilityCooldown);
        canFire = true;
    }
}
