using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHUD : MonoBehaviour
{
    [FoldoutGroup("Dependencies", expanded: true)]
    [FoldoutGroup("Dependencies")][SerializeField] Hitable playerHitable;

    ProgressBar healthBar;

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        healthBar = root?.Q<ProgressBar>("HealthBar");
    }

    private void Update()
    {
        healthBar.highValue = playerHitable.maxHealth;
        healthBar.value = playerHitable.health;
        healthBar.title = playerHitable.health + "/" + playerHitable.maxHealth;
    }
}
