using System;
using Unity.Mathematics;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class TemporalHealEffect : ATemporalEffect
    {
        [Serializable]
        internal struct HealData
        {
            public int amount;
            public float interval;
            public bool delayFirstTick;
            [HideInInspector] public float timer;
        }

        [SerializeField] private HealData m_healData;

        public override TermDefinition? info => GameManager.Config.GetTermDefinition("add_health_over_time");

        public override EEffectType GetEffectType() => EEffectType.Buff;

        protected override void OnInit()
        {
            m_healData.timer =
                m_healData.delayFirstTick ?
                m_healData.interval :
                0.0f;
        }

        protected override void OnUpdate()
        {
            m_healData.timer = math.max(0.0f, m_healData.timer - Time.deltaTime);

            if (m_healData.timer <= 0.0f)
            {
                m_healData.timer = m_healData.interval;
                m_effectData.target.instance.Heal(m_healData.amount, visualFlags);
            }
        }

        public override ITemporalEffect Clone()
        {
            return new TemporalHealEffect
            {
                m_effectData = m_effectData,
                m_temporalData = m_temporalData,
                m_healData = m_healData
            };
        }

        public override EffectDescription GenerateDescription()
        {
            var desc = base.GenerateDescription();
            desc.details = $"{m_healData.amount} {GameManager.Config.GetTermDefinition(EStat.Health).shortName}/{m_healData.interval:0.#}s";
            return desc;
        }
    }
}
