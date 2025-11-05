using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ItemAddAbilityEffect : AItemEffect
    {
        [SerializeField] private AbilitySheet m_ability = null;

        protected override ItemUsageResult OnUse(Item item, CharacterBase target, EItemLocation location)
        {
            if (!target.HasAbility(m_ability))
            {
                target.AddBonusAbility(m_ability);
                return new()
                {
                    success = true,
                    message = $"You learned the ability {m_ability.displayName}"
                };
            }

            return new ItemUsageResult { success = false };
        }
    }
}
