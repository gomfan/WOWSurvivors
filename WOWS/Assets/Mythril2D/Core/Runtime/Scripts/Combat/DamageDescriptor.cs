using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Flags]
    public enum EDamageFlag
    {
        [HideInInspector] None = 0,
        Critical = 1 << 0,
        Miss = 1 << 1,
        [HideInInspector] All = ~None
    }

    public enum EResolutionBehavior
    {
        Default,
        Always,
        Never
    }

    public enum EDamageType
    {
        None,
        Physical,
        Magical
    }

    public interface IDamageSource
    {
        public GameObject gameObject { get; }
        public Stats stats { get; }
    }

    public struct UnknownDamageSource : IDamageSource
    {
        public GameObject gameObject => null;
        public Stats stats => null;
    }

    [Serializable]
    public struct CharacterDamageSource : IDamageSource
    {
        public static CharacterDamageSource Create(CharacterBase character)
        {
            CharacterDamageSource source = new();
            source.m_character = character;
            source.m_stats = new Stats(character.stats.values);
            return source;
        }

        // We store the attacker as a reference for behaviors that depend on the attacker's instance.
        // i.e. provocation system, or any other system that needs to know where the attack is coming from.
        [SerializeField] private PersistableReference<CharacterBase> m_character;

        // we always prefer using the "cached" stats (stats of the attacker at the time of the attack),
        // instead of the stats of the attacker at the time of the calculation.
        [SerializeField] private Stats m_stats;

        public Stats stats => m_stats;
        public CharacterBase character => m_character.instance ?? null;
        public GameObject gameObject => character ? character.gameObject : null;
    }

    /// <summary>
    /// Damage settings
    /// </summary>
    [Serializable]
    public struct DamageDescriptor
    {
        public EDamageType damageType;
        [Min(0)] public float scalingFactor;
        [Min(0)] public int flatDamages;
        public EResolutionBehavior criticalBehavior;
        public EResolutionBehavior missBehavior;
        public bool ignoreDefense;
        public bool silent;
    }

    /// <summary>
    /// Damages output by the attacker after calculations (attack/critical)
    /// </summary>
    [Serializable]
    public struct DamageOutputDescriptor
    {
        [SerializeReference] public IDamageSource source;
        public EDamageType type;
        public int damage;
        public EDamageFlag flags;
        public EResolutionBehavior missBehavior;
        public bool ignoreDefense;
        public bool silent;
    }

    /// <summary>
    /// Damages received by the target after mitigation (defense/miss)
    /// </summary>
    public struct DamageInputDescriptor
    {
        [SerializeReference] public IDamageSource source;
        public int damage;
        public EDamageFlag flags;
        public bool silent;
    }
}