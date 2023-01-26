using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHUD : MonoBehaviour
{
    [FoldoutGroup("Dependencies", expanded: true)]
    [FoldoutGroup("Dependencies")][SerializeField, Required] Logger logger;
    [FoldoutGroup("Dependencies")][SerializeField, Required] Hitable playerHitable;
    [FoldoutGroup("Dependencies")][SerializeField, Required] WeaponController playerWeapon;

    ProgressBar healthBar;

    WaveSpawner waveSpawner;
    Label waveCounter;
    Label waveTimer;
    Label ammoCount;

    private void Start()
    {
        logger = logger == null ? Logger.GetDefaultLogger() : logger;

        if (playerHitable == null) logger.LogWarning("Player hitable not found", this);
        if (playerWeapon == null) logger.LogWarning("Player weapon not found", this);

        var root = GetComponent<UIDocument>().rootVisualElement;

        healthBar = root?.Q<ProgressBar>("HealthBar");
        if (healthBar == null) logger.LogWarning("Healthbar not found", this);
        waveCounter = root?.Q<Label>("Wavecounter");
        if (waveCounter == null) logger.LogWarning("Wavecounter not found", this);
        waveTimer = root?.Q<Label>("Wavetimer");
        if (waveTimer == null) logger.LogWarning("Wavetimer not found", this);
        ammoCount = root?.Q<Label>("Ammocount");
        if (ammoCount == null) logger.LogWarning("Ammocount not found", this);

        waveSpawner = FindObjectOfType<WaveSpawner>();
        if (waveSpawner == null) logger.LogWarning("Wave spawner not found", this);
    }

    private void Update()
    {
        UpdateAmmoCount();
        UpdateHealthbar();
        UpdateWaveCounter();
        UpdateWaveTimer();
    }

    private void UpdateAmmoCount()
    {
        if (playerWeapon == null || ammoCount == null) return;

        var newText = playerWeapon.currentAmmo + "/" + playerWeapon.maxAmmo;

        if (playerWeapon.infiniteAmmo) newText = "∞/∞";

        ammoCount.text = newText;
    }

    private void UpdateHealthbar()
    {
        if (playerHitable == null || healthBar == null) return;

        healthBar.highValue = playerHitable.maxHealth;
        healthBar.value = playerHitable.health;
        healthBar.title = playerHitable.health + "/" + playerHitable.maxHealth;
    }

    private void UpdateWaveCounter()
    {
        if (waveSpawner == null || waveCounter == null) return;

        waveCounter.visible = waveSpawner.isActive;

        if (!waveSpawner.isActive) return;

        waveCounter.text = "Wave " + (waveSpawner.currentWave + 1) + "/" + waveSpawner.waveCount;
    }

    private void UpdateWaveTimer()
    {
        if (waveSpawner == null || waveTimer == null) return;

        waveTimer.visible = waveSpawner.isActive;

        if (!waveSpawner.isActive) return;

        float time = 1f + waveSpawner.currentWaveLength - (Time.time - waveSpawner.waveStartingTime);
        var timeSpan = TimeSpan.FromSeconds(time);
        string timeString = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

        waveTimer.text = timeString;

        waveTimer.style.color = time <= 5.99f ? Color.red : new Color(1f, 0.882352948f, 0.945098042f, 255f);
    }
}
