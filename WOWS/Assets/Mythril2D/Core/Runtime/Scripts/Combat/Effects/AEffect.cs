using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Flags]
    public enum EEffectTargetFlags
    {
        [HideInInspector] None = 0,
        Self = 1 << 0,
        Allies = 1 << 1,
        Enemies = 1 << 2,
        Anything = 1 << 3,
        [HideInInspector] All = ~None
    }

    [Flags]
    public enum EEffectVisualFlags
    {
        [HideInInspector] None,
        NoFloatingText = 1 << 0,
        NoCameraShake = 1 << 1,
        [HideInInspector] All = ~None
    }


    [Serializable]
    public abstract class AEffect : IEffect
    {
        [Serializable]
        protected struct EffectData
        {
            public EEffectTargetFlags targetFlags;
            public EEffectInterruptionPolicy interruptionPolicy;
            public EEffectVisualFlags visualFlags;
            [Range(0.0f, 1.0f)] public float failureRate;

            [HideInInspector] public bool initialized;
            [HideInInspector] public PersistableReference<CharacterBase> source;
            [HideInInspector] public PersistableReference<CharacterBase> target;
            [HideInInspector] public Vector2 velocity;
        }

        public bool initialized => m_effectData.initialized;
        public EEffectInterruptionPolicy interruptionPolicy => m_effectData.interruptionPolicy;
        public EEffectVisualFlags visualFlags => m_effectData.visualFlags;

        [SerializeField] protected EffectData m_effectData;

        private bool IsTargetValidBasedOnFlags(CharacterBase target)
        {
            return
                m_effectData.targetFlags.HasFlag(EEffectTargetFlags.Anything) ||
                m_effectData.targetFlags.HasFlag(EEffectTargetFlags.Self) && target == m_effectData.source ||
                m_effectData.targetFlags.HasFlag(EEffectTargetFlags.Allies) && CombatSolver.AreAllies(m_effectData.source, target) ||
                m_effectData.targetFlags.HasFlag(EEffectTargetFlags.Enemies) && CombatSolver.AreEnemies(m_effectData.source, target);
        }

        private bool EvaluateFailure()
        {
            return UnityEngine.Random.value < m_effectData.failureRate;
        }

        public virtual bool IsApplicable(CharacterBase target) =>
            target != null &&
            CombatSolver.CanTarget(m_effectData.source, target) &&
            IsTargetValidBasedOnFlags(target) &&
            !EvaluateFailure();

        public virtual EffectDescription GenerateDescription() => new()
        {
            name = GetType().Name,
            details = string.Empty
        };

        protected virtual void OnInit() { }
        protected virtual bool OnApply() => true;
        protected virtual void OnDeinit() { }

        public void Init(CharacterBase source)
        {
            if (initialized)
            {
                Debug.LogError($"Effect is already initialized");
            }

            Debug.Assert(!initialized, $"Effect is already initialized");
            m_effectData.source = source;
            OnInit();
            m_effectData.initialized = true;
        }

        private Vector2 ExtractVelocityFromImpactSettings(EffectImpactSettings impactSettings)
        {
            return impactSettings.impactDataType switch
            {
                EEffectImpactDataType.Velocity => impactSettings.impactData,
                EEffectImpactDataType.SourcePosition => (Vector2)m_effectData.target.instance.transform.position - impactSettings.impactData,
                _ => Vector2.zero
            };
        }

        public virtual bool Apply(CharacterBase target, EffectImpactSettings? impactSettings = null)
        {
            m_effectData.target = target;

            m_effectData.velocity = impactSettings.HasValue ?
                ExtractVelocityFromImpactSettings(impactSettings.Value) :
                (m_effectData.source.instance != null ? m_effectData.target.instance.transform.position - m_effectData.source.instance.transform.position : Vector2.zero);

            return OnApply();
        }

        public void Deinit()
        {
            OnDeinit();
            Cleanup();
        }

        protected void Cleanup()
        {
            Debug.Assert(initialized, $"Effect isn't initialized");
            m_effectData.source = null;
            m_effectData.target = null;
            m_effectData.velocity = Vector2.zero;
            m_effectData.initialized = false;
        }
    }
}
