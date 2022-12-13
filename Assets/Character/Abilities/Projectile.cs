using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        private AbilityProjectile ability;

        private List<Collider> ignoreObjectList;

        private LayerMask abilityLayerMask;

        private Logger logger;

        private Vector3 startPosition;

        private float tracking;

        private Transform target;

        private Rigidbody body;

        private bool hasHit = false;

        private void Start()
        {
            startPosition = transform.position;
        }

        public void Initialize(AbilityProjectile ability, List<Collider> ignoreObjectList, Logger logger, LayerMask abilityLayerMask, Transform target = null, float tracking = 0f)
        {
            this.ability = ability;
            this.ignoreObjectList = ignoreObjectList;
            this.logger = logger;
            this.abilityLayerMask = abilityLayerMask;
            this.tracking = tracking;
            this.target = target;
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            body.MovePosition(transform.position + transform.forward * ability.projectileSpeed * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, startPosition) > ability.projectileRange)
            {
                OnMaxRange();
            }
            if (target != null && tracking > 0f)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                Vector3 rotation = Vector3.RotateTowards(transform.forward, direction, tracking * Time.deltaTime, 0f);
                body.rotation = Quaternion.LookRotation(rotation);
            }
        }

        public event System.Action<Collider, Transform> onHit;
        private void OnTriggerEnter(Collider other)
        {
            if (hasHit)
            {
                return;
            }
            
            if (ignoreObjectList.Contains(other)) return;

            if (abilityLayerMask != (abilityLayerMask | (1 << other.gameObject.layer))) return;

            hasHit = true;

            logger?.Log("Projectile hit " + other.gameObject.name, other.gameObject);
            onHit.Invoke(other, transform);
            Destroy(gameObject);
        }

        public event System.Action<Vector3> onMaxRange;
        private void OnMaxRange()
        {
            onMaxRange.Invoke(transform.position);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            onHit = null;
            onMaxRange = null;
        }
    }
}
