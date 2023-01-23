using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargerEnemyAi : MonoBehaviour
{
    private enum ChargerEnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Cooldown,
        Stunned
    }

    [FoldoutGroup("Pain")][PropertyRange(0, 1)][Tooltip("Liklihood of getting stunned after a hit.")] public float painThreshold;
    [FoldoutGroup("Pain")][MinValue(0)] public float painLength;
    [FoldoutGroup("Aggro")][MinValue(0)] public float aggroRange;
    [FoldoutGroup("Aggro")][MinValue(0)] public float deaggroRange;
    [FoldoutGroup("Attack")][MinValue(0)] public int damage;
    [FoldoutGroup("Attack")][MinValue(0)] public float attackCooldown;
    [FoldoutGroup("Attack")] public bool ignoreShield;
    [FoldoutGroup("Attack")] public bool ignoreHealth;
    [FoldoutGroup("Attack")] public bool ignoreDamageReduction;


    private bool gotHitThisFrame;
    private float lastPainTime;
    private float lastAttackTime;
    private ChargerEnemyState currentState;
    private Hitable target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
