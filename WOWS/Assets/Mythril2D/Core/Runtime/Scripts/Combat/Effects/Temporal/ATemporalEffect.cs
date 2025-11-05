using System;
using Unity.Mathematics;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public enum EInitialStackBehavior
    {
        None,
        RefreshDuration,
        AddDuration,
        Interrupt
    }

    [Serializable]
    public abstract class ATemporalEffect : AEffect, ITemporalEffect
    {
        [Serializable]
        protected struct TemporalData
        {
            [InfinityFloat] public float duration;
            public string stackableEffectId;
            public EInitialStackBehavior stackBehavior;
            [HideInInspector] public float remainingDuration;
        }

        [SerializeField] protected TemporalData m_temporalData;

        public virtual TermDefinition? info => null;
        public virtual bool completed => m_temporalData.remainingDuration <= 0.0f;
        public string stackableEffectId => m_temporalData.stackableEffectId;
        public float duration => m_temporalData.duration;

        protected override void OnInit() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnCompleted() { }
        protected virtual void OnStacked(ITemporalEffect effect) { }
        public abstract ITemporalEffect Clone();
        public abstract EEffectType GetEffectType();

        public override bool Apply(CharacterBase target, EffectImpactSettings? impactSettings = null)
        {
            if (base.Apply(target, impactSettings))
            {
                m_temporalData.remainingDuration = m_temporalData.duration > 0.0f ? m_temporalData.duration : float.PositiveInfinity;
                m_effectData.target.instance.AddTemporalEffect(this);
                return true;
            }

            return false;
        }

        public void Complete()
        {
            OnCompleted();
            Deinit();
        }

        public virtual bool TryStack(ITemporalEffect effect)
        {
            if (!string.IsNullOrWhiteSpace(effect.stackableEffectId) && !string.IsNullOrWhiteSpace(m_temporalData.stackableEffectId))
            {
                if (effect.stackableEffectId == m_temporalData.stackableEffectId)
                {
                    switch (m_temporalData.stackBehavior)
                    {
                        case EInitialStackBehavior.RefreshDuration:
                            m_temporalData.remainingDuration = math.max(m_temporalData.remainingDuration, effect.duration);
                            break;
                        case EInitialStackBehavior.AddDuration:
                            m_temporalData.remainingDuration += effect.duration;
                            break;
                        case EInitialStackBehavior.Interrupt:
                            m_temporalData.remainingDuration = 0.0f;
                            break;
                    }

                    OnStacked(effect);
                    return true;
                }
            }

            return false;
        }

        public void Update()
        {
            Debug.Assert(m_effectData.initialized, "Effect must be initialized before updating.");
            m_temporalData.remainingDuration = math.max(0.0f, m_temporalData.remainingDuration - Time.deltaTime);
            OnUpdate();
        }

        public override EffectDescription GenerateDescription()
        {
            var desc = base.GenerateDescription();
            desc.name = $"{info.Value.shortName} ({m_temporalData.duration:0.#}s)";
            desc.details = string.Empty;
            return desc;
        }
    }
}
