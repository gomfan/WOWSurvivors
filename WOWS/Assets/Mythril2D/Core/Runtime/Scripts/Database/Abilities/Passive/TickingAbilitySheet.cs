using System.Collections.Generic;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Abilities + nameof(TickingAbilitySheet))]
    public class TickingAbilitySheet : PassiveAbilitySheet
    {
        [Header("Ticking Ability Settings")]
        [SerializeField] private float m_delayBetweenTicks = 1f;

        public float delayBetweenTicks => m_delayBetweenTicks;

        public override void GenerateAdditionalDescriptionLines(List<AbilityDescriptionLine> lines)
        {
            lines.Add(new()
            {
                content = $"Every {m_delayBetweenTicks:0}s Apply:"
            });

            GenerateDescriptionLinesFromEffects(lines, m_effects);
        }
    }
}
