using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class AI : MonoBehaviour
{
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Flee
    }

    [Header("Info")]
    [SerializeField] protected AIState currentState = AIState.Idle;
    public AIState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    [Space]
    [Header("Dependencies")]
    [SerializeField] protected Transform AimTransform;
    [SerializeField] protected WeaponController weaponController;
    [SerializeField] protected LayerMask abilityLayerMask;
    [SerializeField] protected Logger logger;

    [Space]
    [Header("Settings")]
    public Transform target;
    [SerializeField, Range(0f, 20f)] protected float AimSpeed = 4f;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected float aimBias = 10f;
    [SerializeField] protected float fireDuration = 0f;
    [SerializeField] protected float aimProjectionStrength = 0f;

    [Header("Hooks")]
    [SerializeField] protected List<EventHook<Object>> ActivateAI;
    [SerializeField] protected List<EventHook<Object>> DeactivateAI;

    Vector3 previousTargetPosition;

    [SerializeField] protected bool activated = false;

    private void OnEnable()
    {
        ActivateAI.ForEach(e => e.AddListener(Activate));
        DeactivateAI.ForEach(e => e.AddListener(Deactivate));
    }

    private void OnDisable()
    {
        ActivateAI.ForEach(e => e.RemoveListener(Activate));
        DeactivateAI.ForEach(e => e.RemoveListener(Deactivate));
    }

    private void Activate(Object caller)
    {
        activated = true;
    }

    private void Deactivate(Object caller)
    {
        activated = false;
    }

    private void Start()
    {
        if (!logger) logger = Logger.GetDefaultLogger(this);
        if (!target) target = GameObject.FindWithTag("Player").transform;
    }

    protected bool DetectTarget()
    {
        if (target == null) return false;

        // Raycast at target
        RaycastHit hit;
        if (Physics.Raycast(AimTransform.position, target.position - AimTransform.position, out hit, detectionRange, abilityLayerMask))
        {
            if (hit.transform == target)
            {
                return true;
            }
        }

        return false;
    }

    protected bool AimAtTarget()
    {
        if (target == null) return false;

        var targetPosition = target.position;
        var projectedPosition = targetPosition + (targetPosition - previousTargetPosition) * aimProjectionStrength * (targetPosition - transform.position).magnitude;

        // Rotate towards target
        Vector3 direction = (projectedPosition - AimTransform.position).normalized;
        Vector3 rotation = Vector3.RotateTowards(AimTransform.forward, direction, AimSpeed * Time.deltaTime, 0f);
        AimTransform.rotation = Quaternion.LookRotation(rotation);

        previousTargetPosition = target.position;

        if (Vector3.Angle(AimTransform.forward, direction) <= aimBias)
        {
            return true;
        }
        return false;
    }

    protected void Fire()
    {
        // Fire at target
        if (weaponController)
        {
            weaponController.Fire(fireDuration, abilityLayerMask, null, target);
        }
    }
}
