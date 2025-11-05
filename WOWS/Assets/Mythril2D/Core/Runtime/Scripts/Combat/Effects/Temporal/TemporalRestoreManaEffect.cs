using System;
using Unity.Mathematics;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class TemporalRestoreManaEffect : ATemporalEffect
    {
        [Serializable]
        protected struct RestoreManaData
        {
            public int amount;
            public float interval;
            public bool delayFirstTick;
            [HideInInspector] public float timer;
        }

        [SerializeField] protected RestoreManaData m_restoreManaData;

        public override TermDefinition? info => GameManager.Config.GetTermDefinition("add_mana_over_time");

        public override EEffectType GetEffectType() => EEffectType.Buff;

        protected override void OnInit()
        {
            m_restoreManaData.timer =
                m_restoreManaData.delayFirstTick ?
                m_restoreManaData.interval :
                0.0f;
        }

        protected override void OnUpdate()
        {
            m_restoreManaData.timer = math.max(0.0f, m_restoreManaData.timer - Time.deltaTime);

            if (m_restoreManaData.timer <= 0.0f)
            {
                m_restoreManaData.timer = m_restoreManaData.interval;
                m_effectData.target.instance.RecoverMana(m_restoreManaData.amount, visualFlags);
            }
        }

        public override ITemporalEffect Clone()
        {
            return new TemporalRestoreManaEffect
            {
                m_effectData = m_effectData,
                m_temporalData = m_temporalData,
                m_restoreManaData = m_restoreManaData
            };
        }

        public override EffectDescription GenerateDescription()
        {
            var desc = base.GenerateDescription();
            desc.details = $"{m_restoreManaData.amount} {GameManager.Config.GetTermDefinition(EStat.Mana).shortName}/{m_restoreManaData.interval:0.#}s";
            return desc;
        }
    }
}
