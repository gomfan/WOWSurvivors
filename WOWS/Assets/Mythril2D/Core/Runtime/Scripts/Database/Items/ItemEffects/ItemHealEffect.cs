using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ItemHealEffect : AItemEffect
    {
        [SerializeField] private int m_healthToRestore = 1;

        protected override ItemUsageResult OnUse(Item item, CharacterBase target, EItemLocation location)
        {
            if (target.currentStats[EStat.Health] < target.stats[EStat.Health])
            {
                int previousHealth = target.currentStats[EStat.Health];
                target.Heal(m_healthToRestore, EEffectVisualFlags.NoFloatingText);
                int currentHealth = target.currentStats[EStat.Health];
                int diff = currentHealth - previousHealth;

                return new()
                {
                    success = true,
                    message = $"You recover {diff} <health>"
                };
            }

            return new() { success = false };
        }
    }
}
