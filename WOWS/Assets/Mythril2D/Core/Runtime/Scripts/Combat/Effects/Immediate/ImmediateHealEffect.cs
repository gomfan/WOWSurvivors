using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ImmediateHealEffect : AImmediateEffect
    {
        [SerializeField] private int m_healAmount;

        protected override bool OnApply()
        {
            m_effectData.target.instance.Heal(m_healAmount, visualFlags);
            return true;
        }

        public override EffectDescription GenerateDescription()
        {
            var desc = base.GenerateDescription();
            desc.name = GameManager.Config.GetTermDefinition("add_health").shortName;
            desc.details = m_healAmount.ToString();
            return desc;
        }
    }
}
