using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Abilities
{
    public abstract class AbilityAttack : Ability
    {
        [Space]
        [Header("Hit Effect")]
        [SerializeField, AssetsOnly, AssetList(Path = "Abilities/OnHitEffects")]
        protected GameObject onHitEffect;
        [SerializeField, HideIf("OnHitEffectScaleVisible")] protected Vector3 onHitEffectScale = Vector3.one;
        protected bool OnHitEffectScaleVisible => onHitEffect == null;

        [Space]
        [Header("Muzzle Effect")]
        [SerializeField, AssetsOnly, AssetList(Path = "Weapons/WeaponUseEffects")]
        protected GameObject muzzleFlash;
        [SerializeField, HideIf("WeaponEffectScaleVisible")] protected Vector3 weaponEffectScale = Vector3.one;
        protected bool WeaponEffectScaleVisible => muzzleFlash == null;

        [Space]
        [Header("Audio")]
        [SerializeField] protected List<AudioClip> castAudio;
        [SerializeField] protected List<AudioClip> impactAudio;
    }
}
