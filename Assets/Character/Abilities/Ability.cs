using System.Collections.Generic;
using UnityEngine;
using static Hitable;

namespace Abilities
{
    public abstract class Ability : ScriptableObject
    {
        [Header("Info")]
        [SerializeField] public string abilityName;
        [SerializeField] public string abilityDescription;
        [SerializeField] public Sprite abilityIcon;

        [Space]
        [Header("Abilty Settings")]
        [SerializeField] public float abilityCooldown;
        [SerializeField] public float abilityCastTime;
        [SerializeField] public bool canHold;

        [Space]
        [Header("Attack Settings")]
        public int damage;
        public bool ignoreShield;
        public bool ignoreHealth;
        public bool ignoreDamageReduction;

        public abstract void UseAbility(List<Collider> ignores, Transform origin, LayerMask abilityLayerMask, Transform effectOrigin, Logger logger = null, Transform target = null, AudioSource originAudioSource = null);
    }
}
