using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class TemporalSpeedModifierEffect : ATemporalEffect
    {
        [Serializable]
        public struct SpeedModifierData
        {
            public float factor;
            public AnimationCurve customCurve;
            [HideInInspector] public string key;
        }

        [SerializeField] private SpeedModifierData m_speedModifierData;

        public override EEffectType GetEffectType() => m_speedModifierData.factor < 1.0f ? EEffectType.Debuff : EEffectType.Buff;

        public override TermDefinition? info => GameManager.Config.GetTermDefinition(
            m_speedModifierData.factor > 1.0f ?
                "accelerated" :
                "slowed"
        );

        private bool HasCustomCurve() => m_speedModifierData.customCurve.length > 1;

        private float GetSpeed() =>
            HasCustomCurve() ?
            Mathf.Lerp(m_speedModifierData.factor, 1.0f, m_speedModifierData.customCurve.Evaluate(1.0f - m_temporalData.remainingDuration / m_temporalData.duration)) :
            m_speedModifierData.factor;

        protected override bool OnApply()
        {
            // Use a handle system to lock actions
            // Calling LockActions should return a handle/key to revert the lock
            m_speedModifierData.key = m_effectData.target.instance.ApplyMoveSpeedFactor(GetSpeed());
            return true;
        }

        protected override void OnUpdate()
        {
            if (HasCustomCurve())
            {
                m_effectData.target.instance.UpdateMoveSpeedFactor(m_speedModifierData.key, GetSpeed());
            }
        }

        protected override void OnCompleted()
        {
            m_effectData.target.instance.RemoveMoveSpeedFactor(m_speedModifierData.key);
        }

        public override ITemporalEffect Clone()
        {
            return new TemporalSpeedModifierEffect
            {
                m_effectData = m_effectData,
                m_temporalData = m_temporalData,
                m_speedModifierData = m_speedModifierData
            };
        }

        public override EffectDescription GenerateDescription()
        {
            var desc = base.GenerateDescription();
            desc.details = $"{GameManager.Config.GetTermDefinition("move_speed").shortName} x{m_speedModifierData.factor:0.#}";
            return desc;
        }
    }
}
