using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using static Hitable;

namespace Abilities
{
    [CreateAssetMenu(fileName = "AbilityRaycast", menuName = "Abilities/AbilityRaycast")]
    public class AbilityRaycast : AbilityAttack
    {
        [Space]
        [Header("Raycast Settings")]
        [SerializeField] public float abilityRange;

        [SerializeField, AssetsOnly, AssetList(Path = "Abilities/OnHitEffects")]

        public event System.Action<RaycastHit> onHit = delegate { };
        public override void UseAbility(List<Collider> ignores, Transform origin, LayerMask abilityLayerMask, Transform effectOrigin, Logger logger = null, Transform target = null)
        {
            if (muzzleFlash) // Create cast effect
            {
                var effect = Instantiate(muzzleFlash, effectOrigin.position, effectOrigin.rotation);
                effect.transform.SetParent(effectOrigin);
                effect.transform.localScale = weaponEffectScale;
            }

            if (castAudio) // Play cast audio
            {
                AudioSource.PlayClipAtPoint(castAudio, origin.position);
            }

            // Raycast to find hitable objects
            List<Hitable> hits = new List<Hitable>();
            RaycastHit hit;
            if (Physics.Raycast(origin.position, origin.forward, out hit, abilityRange, abilityLayerMask))
            {
                onHit.Invoke(hit);
                Hitable hitable = hit.collider.GetComponentInParent<Hitable>();
                if (hitable) hits.Add(hitable); // Add hitable to list
                if (onHitEffect) // Create hit effect
                {
                    var effect = Instantiate(onHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    if (impactAudio) // Play impact audio
                    {
                        AudioSource.PlayClipAtPoint(impactAudio, hit.point);
                    }
                    effect.transform.localScale = onHitEffectScale;
                }

                if (logger && hitable) logger.Log($"Hit {hitable.name}");
            }

            Hit(hits, this, logger);
        }

        private void Hit(List<Hitable> targets, Ability ability, Logger logger = null)
        {
            foreach (Hitable target in targets)
            {
                target.Hit(ability);
                if (logger) logger.Log($"Hit {target.name}");
            }
        }
    }
}
