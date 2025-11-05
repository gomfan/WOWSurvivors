using System;
using Unity.Mathematics;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class TemporalDamageEffect : ATemporalEffect
    {
        [Serializable]
        protected struct DamageData
        {
            public DamageDescriptor damage;
            public float interval;
            public bool delayFirstTick;
            [HideInInspector] public float timer;
            [HideInInspector] public DamageOutputDescriptor damageOutput;
        }

        [SerializeField] private DamageData m_damageData;

        public override TermDefinition? info => GameManager.Config.GetTermDefinition("damage_over_time");

        public override EEffectType GetEffectType() => EEffectType.Debuff;

        protected override void OnInit()
        {
            m_damageData.damageOutput = DamageSolver.SolveDamageOutput(m_effectData.source, m_damageData.damage);
        }

        protected override void OnDeinit()
        {
            m_damageData.damageOutput = default;
        }

        protected override bool OnApply()
        {
            m_damageData.timer =
                m_damageData.delayFirstTick ?
                m_damageData.interval :
                0.0f;

            return true;
        }

        protected override void OnUpdate()
        {
            m_damageData.timer = math.max(0.0f, m_damageData.timer - Time.deltaTime);

            if (m_damageData.timer <= 0.0f)
            {
                m_damageData.timer = m_damageData.interval;
                m_effectData.target.instance.Damage(m_damageData.damageOutput, visualFlags, m_effectData.velocity);
            }
        }

        public override ITemporalEffect Clone()
        {
            return new TemporalDamageEffect
            {
                m_effectData = m_effectData,
                m_temporalData = m_temporalData,
                m_damageData = m_damageData
            };
        }

        public override EffectDescription GenerateDescription()
        {
            string flatDamage = m_damageData.damage.flatDamages != 0.0f ? $"{m_damageData.damage.flatDamages:0.#} {GameManager.Config.GetTermDefinition("flat_damage").shortName}" : string.Empty;
            string scaledDamage = m_damageData.damage.scalingFactor != 0.0f ? $"{m_damageData.damage.scalingFactor:0.#} {GameManager.Config.GetTermDefinition("scaled_damage").shortName}" : string.Empty;

            var desc = base.GenerateDescription();
            desc.details = $"{flatDamage}{(string.IsNullOrEmpty(flatDamage) || string.IsNullOrEmpty(scaledDamage) ? string.Empty : "+")}{scaledDamage} {GameManager.Config.GetTermDefinition(m_damageData.damage.damageType).shortName}/{m_damageData.interval:0.#}s";
            return desc;
        }
    }
}
