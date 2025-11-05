using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ImmediateDamageEffect : AImmediateEffect
    {
        [SerializeField] private DamageDescriptor m_damageDescriptor;

        private DamageOutputDescriptor? m_damageOutput;

        protected override void OnInit()
        {
            m_damageOutput = DamageSolver.SolveDamageOutput(m_effectData.source, m_damageDescriptor);
        }

        protected override bool OnApply()
        {
            return m_effectData.target.instance.Damage(m_damageOutput.Value, visualFlags, m_effectData.velocity);
        }

        protected override void OnDeinit()
        {
            m_damageOutput = null;
        }

        public override EffectDescription GenerateDescription()
        {
            var desc = base.GenerateDescription();

            string flatDamage = m_damageDescriptor.flatDamages != 0.0f ? $"{m_damageDescriptor.flatDamages:0.#} {GameManager.Config.GetTermDefinition("flat_damage").shortName}" : string.Empty;
            string scaledDamage = m_damageDescriptor.scalingFactor != 0.0f ? $"{m_damageDescriptor.scalingFactor:0.#} {GameManager.Config.GetTermDefinition("scaled_damage").shortName}" : string.Empty;

            desc.name = GameManager.Config.GetTermDefinition("remove_health").shortName;
            desc.details = $"{flatDamage}{(string.IsNullOrEmpty(flatDamage) || string.IsNullOrEmpty(scaledDamage) ? string.Empty : "+")}{scaledDamage} {GameManager.Config.GetTermDefinition(m_damageDescriptor.damageType).shortName}";
            return desc;
        }
    }
}
