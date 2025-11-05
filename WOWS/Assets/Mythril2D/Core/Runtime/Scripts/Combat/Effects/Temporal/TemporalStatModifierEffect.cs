using System;
using Unity.Mathematics;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class TemporalStatModifierEffect : ATemporalEffect
    {
        [Serializable]
        internal struct StatBoostEffect
        {
            public int amount;
            public EStat stat;
        }

        [SerializeField] private StatBoostEffect m_statBoostData;

        public override TermDefinition? info =>
            m_statBoostData.amount > 0 ?
                GameManager.Config.GetStatIncreaseTermDefinition(m_statBoostData.stat) :
                GameManager.Config.GetStatDecreaseTermDefinition(m_statBoostData.stat);

        public override EEffectType GetEffectType() => m_statBoostData.amount < 0 ? EEffectType.Debuff : EEffectType.Buff;

        protected override bool OnApply()
        {
            m_effectData.target.instance.currentStats[m_statBoostData.stat] += m_statBoostData.amount;
            return true;
        }

        protected override void OnCompleted()
        {
            int amountToRemove = m_statBoostData.amount;

            // If the target is dead, we can't remove stats, so we skip this step
            if (m_effectData.target.instance.dead)
            {
                return;
            }

            // Ensure we don't kill the target after the effect ends
            if (m_statBoostData.stat == EStat.Health)
            {
                amountToRemove = math.min(amountToRemove, m_effectData.target.instance.currentStats[EStat.Health] - 1);
            }

            // Ensure we don't leave the target with negative mana after the effect ends
            if (m_statBoostData.stat == EStat.Mana)
            {
                amountToRemove = math.min(amountToRemove, m_effectData.target.instance.currentStats[EStat.Mana]);
            }

            m_effectData.target.instance.currentStats[m_statBoostData.stat] -= amountToRemove;
        }

        public override ITemporalEffect Clone()
        {
            return new TemporalStatModifierEffect
            {
                m_effectData = m_effectData,
                m_temporalData = m_temporalData,
                m_statBoostData = m_statBoostData
            };
        }

        public override EffectDescription GenerateDescription()
        {
            var desc = base.GenerateDescription();
            string prefix = m_statBoostData.amount > 0 ? "+" : string.Empty;
            desc.details = $"{prefix}{m_statBoostData.amount} {GameManager.Config.GetTermDefinition(m_statBoostData.stat).shortName}";
            return desc;
        }
    }
}
