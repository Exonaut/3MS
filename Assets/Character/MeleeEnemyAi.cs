using Sirenix.OdinInspector;
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

    [FoldoutGroup("Pain")][PropertyRange(0, 1)][Tooltip("Liklihood of getting stunned after a hit.")] public float painThreshold;
    [FoldoutGroup("Pain")][MinValue(0)] public float painLength;
    //[MinValue(0)] public float reactionDelay; // implement this
    [FoldoutGroup("Aggro")][MinValue(0)] public float aggroRange;
    [FoldoutGroup("Aggro")][MinValue(0)] public float deaggroRange;
    [FoldoutGroup("Attack")][MinValue(0)] public float attackRange;
    [FoldoutGroup("Attack")][MinValue(0)] public int damage;
    [FoldoutGroup("Attack")][MinValue(0)] public float attackCooldown;
    [FoldoutGroup("Attack")] public bool ignoreShield;
    [FoldoutGroup("Attack")] public bool ignoreHealth;
    [FoldoutGroup("Attack")] public bool ignoreDamageReduction;



    private NavMeshAgent agent;
    private bool gotHitThisFrame;
    private float lastPainTime;
    private float lastAttackTime;
    private MeleeEnemyState currentState;
    private Hitable target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        gotHitThisFrame = false;
        currentState = MeleeEnemyState.Idle;
        target = null;

        GetComponent<Hitable>().onDamage += _ => gotHitThisFrame = true;
    }

    private void Update()
    {
        if (gotHitThisFrame)
        {
            gotHitThisFrame = false;
            if (Random.Range(0f, 1f) < painThreshold)
            {
                currentState = MeleeEnemyState.Stunned;
                lastPainTime = Time.time;
                agent.destination = transform.position;
            }
        }
        switch (currentState)
        {
            case MeleeEnemyState.Chasing:
                if (target == null || Vector3.Distance(target.transform.position, transform.position) > deaggroRange)
                    currentState = MeleeEnemyState.Idle;
                else if (Vector3.Distance(target.transform.position, transform.position) < attackRange)
                    currentState = MeleeEnemyState.Attacking;
                else
                    agent.destination = target.transform.position + 0.8f * attackRange * (transform.position - target.transform.position).normalized;
                break;
            case MeleeEnemyState.Attacking:
                if (Vector3.Distance(target.transform.position, transform.position) < attackRange)
                {
                    target.Hit(damage, ignoreShield, ignoreHealth, ignoreDamageReduction);
                    lastAttackTime = Time.time;
                    currentState = MeleeEnemyState.Cooldown;
                }
                else
                    currentState = MeleeEnemyState.Chasing;

                break;
            case MeleeEnemyState.Cooldown:
                if (Time.time - lastAttackTime >= attackCooldown)
                    currentState = MeleeEnemyState.Attacking;
                break;
            case MeleeEnemyState.Stunned:
                if (Time.time - lastPainTime >= painLength)
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
