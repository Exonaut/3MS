using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Exo.Events;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hitable : MonoBehaviour
{
    public static float tickRate = 1f;

    [FoldoutGroup("Dependencies", expanded: true)]
    [FoldoutGroup("Dependencies")][SerializeField, Required] private Logger logger;

    [FoldoutGroup("Attributes", expanded: true)]
    [FoldoutGroup("Attributes")][SerializeField, MinValue(0)] public int maxHealth = 100;
    [FoldoutGroup("Attributes")][SerializeField, PropertyRange(0, "maxHealth")] public int health;
    [FoldoutGroup("Attributes")][SerializeField, MinValue(0)] public int maxShield = 20;
    [FoldoutGroup("Attributes")][SerializeField, PropertyRange(0, "maxShield")] public int shield;
    [FoldoutGroup("Attributes")][SerializeField, MinValue(0)] private int damageReduction = 0;
    [FoldoutGroup("Attributes")][SerializeField, MinValue(0)] private float damageMultiplier = 1;
    [FoldoutGroup("Attributes")][SerializeField, MinValue(0)] private int shieldRechargeRate = 4;
    [FoldoutGroup("Attributes")][SerializeField, MinValue(0)] private float shieldRechargeDelay = 3;

    [FoldoutGroup("Attributes")][Tooltip("Instance dies when getting hit regardless of damage")] public bool fragile = false;
    [FoldoutGroup("Attributes")][Tooltip("Instance can not die or loose health. Overrules fragile")] public bool invincible = false;

    [FoldoutGroup("Status Settings", expanded: true)]

    [FoldoutGroup("Death Effects", expanded: true)]
    [FoldoutGroup("Death Effects")][SerializeField, AssetsOnly, AssetList(Path = "Characters/DeathEffects")] private GameObject deathEffect;
    [FoldoutGroup("Death Effects")][SerializeField, AssetsOnly, AssetList(Path = "Characters/DeathEffects")] private AudioClip deathSound;
    [FoldoutGroup("Death Effects")][SerializeField] private Transform deathEffectOrigin;

    [FoldoutGroup("Event Hooks", true)][SerializeField, SceneObjectsOnly, Required] List<EventHook> enableHook;
    [FoldoutGroup("Event Hooks", true)][SerializeField, SceneObjectsOnly, Required] List<EventHook> disableHook;
    [FoldoutGroup("Event Hooks", true)][SerializeField, SceneObjectsOnly, Required] List<EventHook> restartHook;

    private float shieldRechargeDelayTimer = 0;

    private static int frostDamage = 30;
    private static float bleedDamage = 0.2f;
    private static int burnDamage = 5;
    private static int poisonDamage = 1;
    private static float rotDamage = 0.02f;
    private static float shockDamage = 0.2f;
    private static int shockHealthDamage = 2;

    public void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        enableHook?.ForEach(hook => hook.AddListener(Enable));
        disableHook?.ForEach(hook => hook.AddListener(Disable));
        restartHook?.ForEach(hook => hook.AddListener(Restart));

        StartCoroutine(Tick());
    }

    private void OnDisable()
    {
        enableHook?.ForEach(hook => hook.RemoveListener(Enable));
        disableHook?.ForEach(hook => hook.RemoveListener(Disable));
        restartHook?.ForEach(hook => hook.RemoveListener(Restart));

        StopCoroutine(Tick());
    }

    public void Initialize()
    {
        health = maxHealth;
        shield = maxShield;

        if (!logger) logger = Logger.GetDefaultLogger(this);
    }

    private void Update()
    {
        if (shieldRechargeDelayTimer > 0)
        {
            shieldRechargeDelayTimer -= Time.deltaTime;
        }

        Kill();
    }

    IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickRate);
            if (shieldRechargeDelayTimer <= 0)
            {
                shieldRechargeDelayTimer = 0;
                RechargeShield();
            }
        }
    }

    private void Rot()
    {
        Damage((int)(maxHealth * rotDamage), true);
    }

    private void Poison()
    {
        Damage(poisonDamage, true);
    }

    private void Burn()
    {
        Damage(burnDamage, true);
    }

    private void Shock()
    {
        if (shield > 0)
        {
            Damage((int)(maxShield * shockDamage), false, true);
        }
        else
        {
            Damage(shockHealthDamage);
        }
    }

    private void Bleed()
    {
        Damage((int)(bleedDamage * maxHealth), true);
    }

    private void Frost()
    {
        Damage(frostDamage);
    }

    public readonly HookableEvent onShieldRecharged = new HookableEvent("ShieldRecharged");
    private void RechargeShield()
    {
        if (shield < maxShield)
        {
            shield = Mathf.Min(shield + shieldRechargeRate, maxShield);
            if (shield == maxShield)
            {
                onShieldRecharged?.Invoke(this);
            }
        }
    }

    public readonly HookableEvent onShieldRechargeDelay = new HookableEvent("ShieldRechargeDelay");
    private void SuspendShieldRecharge()
    {
        shieldRechargeDelayTimer = shieldRechargeDelay;
        onShieldRechargeDelay?.Invoke(this);
    }

    public readonly HookableEvent onHit = new HookableEvent("Hit");
    public readonly HookableEvent onShieldHit = new HookableEvent("ShieldHit");
    public readonly HookableEvent onHealthHit = new HookableEvent("HealthHit");
    public void Hit(int damage, bool ignoreShield = false, bool ignoreHealth = false, bool ignoreDamageReduction = false)
    {
        logger.Log($"{gameObject.name} was hit for {damage} damage", this);

        if (!invincible)
        {
            onHit?.Invoke(this);
            if (shield > 0 && !ignoreShield)
            {
                onShieldHit?.Invoke(this);
            }
            if (!ignoreHealth)
            {
                onHealthHit?.Invoke(this);
            }
        }

        Damage(damage, ignoreShield, ignoreHealth, ignoreDamageReduction);
    }

    public void Hit(Ability attack)
    {
        Hit(attack.damage);
    }

    public readonly HookableEvent onDamage = new HookableEvent("Damage");
    public readonly HookableEvent onHealthDamage = new HookableEvent("HealthDamage");
    public readonly HookableEvent onShieldDamage = new HookableEvent("ShieldDamage");
    protected void Damage(int damage, bool ignoreShield = false, bool ignoreHealth = false, bool ignoreDamageReduction = false)
    {
        if (invincible) return;

        if (fragile)
        {
            Die();
            return;
        }

        if (!ignoreDamageReduction)
        {
            damage -= damageReduction;
        }

        int remainingDamage = (int)Mathf.Max(damage * damageMultiplier, 0);

        print($"{gameObject.name} took {remainingDamage} damage");

        int totalDamage = remainingDamage;
        int totalShieldDamage = 0;
        int totalHealthDamage = 0;

        if (!ignoreShield && shield > 0)
        {
            shield -= remainingDamage;
            totalShieldDamage = remainingDamage - shield;
            if (!ignoreHealth && shield < 0)
            {
                health += shield;
                shield = 0;
                totalHealthDamage = -shield;
            }
            onDamage?.Invoke(this);
        }
        else if (!ignoreHealth)
        {
            health -= remainingDamage;
            totalHealthDamage = remainingDamage;
            onDamage?.Invoke(this);
        }

        if (!ignoreHealth)
        {
            onHealthDamage?.Invoke(this);
        }

        if (!ignoreShield)
        {
            SuspendShieldRecharge();
            onShieldDamage?.Invoke(this);
        }

        Kill();
    }

    public void Kill()
    {
        if ((!invincible) && (health <= 0))
        {
            Die();
        }
    }

    public readonly HookableEvent onDie = new HookableEvent("Die");
    protected virtual void Die()
    {
        logger.Log($"{gameObject.name} died", this);
        onDie.Invoke(this);
        if (deathEffect)
        {
            if (!deathEffectOrigin)
            {
                logger.LogWarning($"{gameObject.name} has no death effect origin", this);
                deathEffectOrigin = transform;
            }
            var i = Instantiate(deathEffect, deathEffectOrigin.position, Quaternion.identity);
            i.transform.localScale = deathEffectOrigin.localScale;
        }
        else
        {
            logger.LogWarning($"{gameObject.name} has no death effect", this);
        }

        if (deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, deathEffectOrigin.position);
        }
        else
        {
            logger.LogWarning($"{gameObject.name} has no death sound", this);
        }

        if (!gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
        else
        {
            maxHealth = (int)(maxHealth * 1.1f);
            health = maxHealth;
        }
    }

    public void Restart()
    {
        health = maxHealth;
        shield = maxShield;
        enabled = true;
        gameObject.SetActive(true);
    }

    void Restart(MonoBehaviour sender)
    {
        Restart();
    }

    void Enable()
    {
        logger.Log("{gameObject.name} enabled", this);
        gameObject.SetActive(true);
    }

    void Enable(MonoBehaviour sender)
    {
        Enable();
    }

    void Disable()
    {
        logger.Log($"{gameObject.name} disabled", this);
        gameObject.SetActive(false);
        print("Disable" + gameObject.name);
    }

    void Disable(MonoBehaviour sender)
    {
        Disable();
    }
}
