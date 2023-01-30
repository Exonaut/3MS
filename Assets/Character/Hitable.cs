using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

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

    [FoldoutGroup("Death Effects", expanded: true)]
    [FoldoutGroup("Death Effects")][SerializeField, AssetsOnly] private GameObject deathEffect;
    [FoldoutGroup("Death Effects")][SerializeField, AssetsOnly] private AudioClip deathSound;
    [FoldoutGroup("Death Effects")][SerializeField] private Transform deathEffectOrigin;

    [FoldoutGroup("Event Hooks", true)][SerializeField, SceneObjectsOnly, Required] List<EventHook<Object>> enableHook;
    [FoldoutGroup("Event Hooks", true)][SerializeField, SceneObjectsOnly, Required] List<EventHook<Object>> disableHook;
    [FoldoutGroup("Event Hooks", true)][SerializeField, SceneObjectsOnly, Required] List<EventHook<Object>> restartHook;

    public bool HasFullHealth => health == maxHealth;

    private static float DeathPlaneYCoordinate => -20;
    private float shieldRechargeDelayTimer = 0;

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
        if (!deathEffectOrigin)
        {
            deathEffectOrigin = transform;
            logger.LogWarning($"{gameObject.name} has no death effect origin", this);
        }
    }

    private void Update()
    {
        if (shieldRechargeDelayTimer > 0)
        {
            shieldRechargeDelayTimer -= Time.deltaTime;
        }

        CheckKill();
    }

    public event Action<Hitable> onTick;
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
            onTick?.Invoke(this);
        }
    }
    public event Action<Hitable> onShieldRecharged;
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

    public event Action<Hitable> onSuspendShieldRecharge;
    private void SuspendShieldRecharge()
    {
        shieldRechargeDelayTimer = shieldRechargeDelay;
        onSuspendShieldRecharge?.Invoke(this);
    }

    [Hookable] public event Action<Hitable> onHit;
    [Hookable] public event Action<Hitable> onShieldHit;
    [Hookable] public event Action<Hitable> onHealthHit;
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

    [Hookable] public event Action<Hitable> onDamage;
    [Hookable] public event Action<Hitable> onHealthDamage;
    [Hookable] public event Action<Hitable> onShieldDamage;
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

        logger?.Log($"{gameObject.name} took {remainingDamage} damage", this);

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

        CheckKill();
    }

    public void CheckKill()
    {
        if (((!invincible) && (health <= 0)) || transform.position.y < DeathPlaneYCoordinate)
            Die();
    }

    [Hookable] public event Action<Hitable> onDie;
    protected virtual void Die()
    {
        logger.Log($"{gameObject.name} died", this);
        onDie?.Invoke(this);
        if (deathEffect)
        {
            var i = Instantiate(deathEffect, deathEffectOrigin.position, Quaternion.identity);
            i.transform.localScale = deathEffectOrigin.localScale;
        }
        else
        {
            logger.LogWarning($"{gameObject.name} has no death effect", this);
        }

        if (deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        else
        {
            logger.LogWarning($"{gameObject.name} has no death sound", this);
        }

        gameObject.SetActive(false);
    }

    public void Heal(int damage)
    {
        health = Math.Min(health + damage, maxHealth);
    }

    [Hookable] public event Action<Hitable> onRestart;
    public void Restart(Object sender = null)
    {
        health = maxHealth;
        shield = maxShield;
        enabled = true;
        gameObject.SetActive(true);
        onRestart?.Invoke(this);
    }

    void Enable(Object sender = null)
    {
        logger.Log("{gameObject.name} enabled", this);
        gameObject.SetActive(true);
    }

    void Disable(Object sender = null)
    {
        logger.Log($"{gameObject.name} disabled", this);
        gameObject.SetActive(false);
        print("Disable" + gameObject.name);
    }
}
