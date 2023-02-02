using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossAi : MonoBehaviour
{
    public GameObject FireballPrefab;
    public Transform AimOrigin;

    [FoldoutGroup("Attack")][MinValue(0)] public int damage;
    [FoldoutGroup("Attack")][MinValue(0)] public float attackCooldown;
    [FoldoutGroup("Attack")][MinValue(0)] public float homingParameter;
    [FoldoutGroup("Attack")][MinValue(0)] public float fireballSpeed;
    [FoldoutGroup("Attack")][MinValue(0)] public float lifetime;

    private NavMeshAgent agent;
    private float lastAttackTime;
    private Hitable target;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        target = null;
        animator = GetComponent<Animator>();

        GetComponent<Hitable>().onDie += (caller) =>
        {
            GameGlobals.gameWon = true;
            SceneManager.LoadScene("EndGameScene");
        };
    }

    private void Update()
    {
        if (target == null)
        {
            var players = GameObject
                        .FindGameObjectsWithTag("Player")
                        .Select(player => player.GetComponent<Hitable>());
            if (players.Any())
                target = players.First();
        }
        else
        {
            agent.destination = target.transform.position + 0.8f * (transform.position - target.transform.position).normalized;
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                ShootFireball(target.transform);
            }
        }
    }

    private void ShootFireball(Transform target)
    {
        var shot = Instantiate(FireballPrefab, AimOrigin.position, Quaternion.identity, null);
        var fireballAi = shot.GetComponent<FireballAi>();
        fireballAi.Initialize(target, damage, homingParameter, fireballSpeed, lifetime);
    }
}
