using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ImmediateRestoreManaEffect : AImmediateEffect
    {
        [SerializeField] private int m_manaToRestore;

        protected override bool OnApply()
        {
            m_effectData.target.instance.RecoverMana(m_manaToRestore, visualFlags);
            return true;
        }

        public override EffectDescription GenerateDescription()
        {
            var desc = base.GenerateDescription();
            desc.name = GameManager.Config.GetTermDefinition("add_mana").shortName;
            desc.details = m_manaToRestore.ToString();
            return desc;
        }
    }
}
