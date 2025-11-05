using System.Collections.Generic;
using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    public struct AbilityDescriptionLine
    {
        public string header;
        public string content;
    }

    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Abilities + nameof(ActiveAbilitySheet))]
    public class ActiveAbilitySheet : AbilitySheet
    {
        [Header("Audio")]
        [SerializeField] private AudioClipResolver m_fireAudio;

        [Header("Active Ability Settings")]
        [SerializeField] private int m_manaCost = 0;
        [SerializeField] private float m_cooldown = 0.0f;
        [SerializeField] private bool m_canInterupt = false;
        [SerializeField] private EActionFlags m_disabledActionsWhileCasting = EActionFlags.All;
        [SerializeField] private bool m_updateLookAtDirectionOnFire = true;
        [SerializeReference, SubclassSelector] private IEffect[] m_autoAppliedEffectsToCasterOnFire;

        public bool canInterupt => m_canInterupt;
        public int manaCost => m_manaCost;
        public float cooldown => m_cooldown;
        public EActionFlags disabledActionsWhileCasting => m_disabledActionsWhileCasting;
        public bool updateLookAtDirectionOnFire => m_updateLookAtDirectionOnFire;
        public AudioClipResolver fireAudio => m_fireAudio;
        public IEnumerable<IEffect> autoAppliedEffectsToCasterOnFire => m_autoAppliedEffectsToCasterOnFire;

        public override void GenerateAdditionalDescriptionLines(List<AbilityDescriptionLine> lines)
        {
            base.GenerateAdditionalDescriptionLines(lines);

            if (m_manaCost > 0)
            {
                lines.Add(new AbilityDescriptionLine
                {
                    header = GameManager.Config.GetTermDefinition("mana_cost").shortName,
                    content = m_manaCost.ToString()
                });
            }

            if (m_cooldown > 0.0f)
            {
                lines.Add(new AbilityDescriptionLine
                {
                    header = GameManager.Config.GetTermDefinition("cooldown").shortName,
                    content = $"{m_cooldown:0}s"
                });
            }

            GenerateDescriptionLinesFromEffects(lines, m_autoAppliedEffectsToCasterOnFire);
        }
    }
}
