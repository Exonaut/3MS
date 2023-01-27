using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MeleeEnemyAi : MonoBehaviour
{
    private enum MeleeEnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Cooldown,
        Stunned
    }

    //[MinValue(0)] public float reactionDelay; // implement this
    [FoldoutGroup("Aggro")][MinValue(0)] public float aggroRange;
    [FoldoutGroup("Aggro")][MinValue(0)] public float deaggroRange;
    [FoldoutGroup("Attack")][MinValue(0)] public float attackRange;
    [FoldoutGroup("Attack")][MinValue(0)] public int damage;
    [FoldoutGroup("Attack")][MinValue(0)] public float attackLength;
    [FoldoutGroup("Attack")][MinValue(0)] public float attackCooldown;
    [FoldoutGroup("Attack")] public bool ignoreShield;
    [FoldoutGroup("Attack")] public bool ignoreHealth;
    [FoldoutGroup("Attack")] public bool ignoreDamageReduction;



    private NavMeshAgent agent;
    private Painable painable;
    private float lastWindupTime;
    private float lastAttackTime;
    private MeleeEnemyState currentState;
    private Hitable target;
    private Animator animator;

    private bool TargetInDistance => Vector3.Distance(target.transform.position, transform.position) < attackRange;
    private bool IsFacingTarget => Vector3.Dot(target.transform.position - transform.position, transform.forward) > 0;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = MeleeEnemyState.Idle;
        target = null;
        painable = GetComponent<Painable>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (painable != null && painable.IsInPain)
            currentState = MeleeEnemyState.Stunned;

        Console.WriteLine(currentState);
        switch (currentState)
        {
            case MeleeEnemyState.Chasing:
                if (target == null || Vector3.Distance(target.transform.position, transform.position) > deaggroRange)
                    currentState = MeleeEnemyState.Idle;
                else if (TargetInDistance)
                {
                    currentState = MeleeEnemyState.Attacking;
                    animator.SetTrigger("attackTrigger");
                    lastWindupTime = Time.time;
                }
                else
                    agent.destination = target.transform.position + 0.8f * attackRange * (transform.position - target.transform.position).normalized;
                break;
            case MeleeEnemyState.Attacking:
                if (Time.time - lastWindupTime >= attackLength)
                {
                    lastAttackTime = Time.time;
                    if (TargetInDistance && IsFacingTarget)
                        target.Hit(damage, ignoreShield, ignoreHealth, ignoreDamageReduction);
                    currentState = MeleeEnemyState.Cooldown;
                }
                break;
            case MeleeEnemyState.Cooldown:
                if (Time.time - lastAttackTime >= attackCooldown)
                    currentState = MeleeEnemyState.Chasing;
                break;
            case MeleeEnemyState.Stunned:
                agent.destination = transform.position;
                if (!painable.IsInPain)
                    currentState = MeleeEnemyState.Chasing;
                break;
            case MeleeEnemyState.Idle:
            default:
                var players = GameObject
                    .FindGameObjectsWithTag("Player")
                    .Select(player => player.GetComponent<Hitable>())
                    .Where(player => player != null && Vector3.Distance(player.transform.position, transform.position) < aggroRange);
                if (players.Any())
                {
                    target = players.First();
                    currentState = MeleeEnemyState.Chasing;
                }
                break;
        }
    }
}
