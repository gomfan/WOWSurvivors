using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ItemRestoreManaEffect : AItemEffect
    {
        [SerializeField] private int m_manaToRestore = 1;

        protected override ItemUsageResult OnUse(Item item, CharacterBase target, EItemLocation location)
        {
            if (target.currentStats[EStat.Mana] < target.stats[EStat.Mana])
            {
                int previousMana = target.currentStats[EStat.Mana];
                target.RecoverMana(m_manaToRestore, EEffectVisualFlags.NoFloatingText);
                int currentMana = target.currentStats[EStat.Mana];
                int diff = currentMana - previousMana;

                return new()
                {
                    success = true,
                    message = $"You recover {diff} <mana>"
                };
            }

            return new() { success = false };
        }
    }
}
