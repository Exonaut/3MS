using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "AbilityProjectile", menuName = "Abilities/AbilityProjectile")]
    public class AbilityProjectile : AbilityAttack
    {
        [Space]
        [Header("Projectile Settings")]
        [SerializeField, AssetsOnly, AssetList(Path = "Abilities/Projectiles")] protected Projectile projectilePrefab;
        [SerializeField][Min(0)] public float projectileRange;
        [SerializeField][Min(0)] public float projectileSpeed;
        [SerializeField][Min(0)] public float projectileTracking = 0;
        public override void UseAbility(List<Collider> ignores, Transform origin, LayerMask abilityLayerMask, Transform effectOrigin, Logger logger = null, Transform target = null, AudioSource originAudioSource = null)
        {
            var projectile = Instantiate(projectilePrefab, origin.position, origin.rotation);
            projectile.Initialize(this, ignores, logger, abilityLayerMask, target, projectileTracking);
            projectile.onHit += OnHit;
            projectile.onMaxRange += OnMaxRange;
        }

        private void OnHit(Collider hit, Transform transform)
        {
            CreateHitEffect(hit, transform);
            Hit(hit);
        }

        private void OnMaxRange(Vector3 position)
        {
            CreateHitEffect(position, Vector3.up);
        }

        private void CreateHitEffect(Collider hit, Transform transform)
        {
            CreateHitEffect(transform.position, transform.forward);
        }

        private void CreateHitEffect(Vector3 position, Vector3 normal)
        {
            if (onHitEffect) // Create hit effect
            {
                var effect = Instantiate(onHitEffect, position, Quaternion.LookRotation(normal));
                if (impactAudio != null) // Play impact audio
                {
                    AudioSource.PlayClipAtPoint(impactAudio[Random.Range(0, impactAudio.Count)], position);
                }
                effect.transform.localScale = onHitEffectScale;
            }
        }

        private void Hit(Collider hit)
        {
            Hitable hitable = hit.GetComponentInParent<Hitable>();
            if (hitable)
            {
                hitable.Hit(this);
            }
            Debug.Log("Hit");
        }
    }
}
