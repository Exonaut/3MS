using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class RangedAI : AI
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField][MinMaxSlider(0, 50, true)] private Vector2Int movementRange;
    [SerializeField][MinMaxSlider(0, 10, true)] private Vector2 retargetTime;
    [SerializeField] Transform body;
    [SerializeField] float bodyTurnSpeed = 30;

    Vector3 targetPosition;
    Coroutine retargetCoroutine;

    // Update is called once per frame
    void Update()
    {
        if (!activated)
        {
            StopAllCoroutines();
            currentState = AIState.Idle;
            return;
        };
        switch (currentState)
        {
            case AIState.Patrol:
                Patrol();
                if (DetectTarget())
                {
                    currentState = AIState.Attack;
                    logger.Log("Target detected, switching to attack state", this);
                }
                break;
            case AIState.Attack:
                Attack();
                if (!DetectTarget())
                {
                    currentState = AIState.Patrol;
                    fireDuration = 0f;
                    logger.Log($"Lost target, switching to idle state", this);
                }
                break;
            default:
                currentState = AIState.Patrol;
                retargetCoroutine = StartCoroutine(Retarget());
                break;
        }
    }

    void Patrol()
    {

    }

    void Attack()
    {
        TurnTowardsTarget();
        if (AimAtTarget())
        {
            Fire();
            fireDuration += Time.deltaTime;
            return;
        }
        fireDuration = 0f;
    }

    void TurnTowardsTarget()
    {
        agent.updateRotation = false;

        if (body == null) return;
        var dir = target.position - body.position;
        dir.y = 0f;
        var q = Quaternion.LookRotation(dir);
        body.rotation = Quaternion.RotateTowards(body.rotation, q, bodyTurnSpeed * Time.deltaTime);
    }

    IEnumerator Retarget()
    {
        while (true)
        {
            var r = Random.insideUnitCircle * Random.Range(movementRange.x, movementRange.y);
            targetPosition.x = transform.position.x + r.x;
            targetPosition.z = transform.position.z + r.y;
            agent.SetDestination(targetPosition);
            yield return new WaitForSeconds(Random.Range(retargetTime.x, retargetTime.y));
        }
    }
}
