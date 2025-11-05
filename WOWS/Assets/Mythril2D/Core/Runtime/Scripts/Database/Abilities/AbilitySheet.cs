using System.Collections.Generic;
using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    public class AbilitySheet : DatabaseEntry, INameable
    {
        public enum EAbilityStateManagementMode
        {
            Automatic,
            AlwaysOn,
            AlwaysOff
        }

        public enum EAbilityOrientationMode
        {
            Static,
            Polydirectional,
            Horizontal
        }

        [Header("UI Settings")]
        [SerializeField] private Sprite m_icon = null;
        [SerializeField] private string m_displayName = string.Empty;
        [SerializeField] private string m_description = string.Empty;

        [Header("References")]
        [SerializeField] private GameObject m_prefab = null;

        [Header("Animation Settings")]
        [SerializeField] private EAbilityOrientationMode m_orientationMode = EAbilityOrientationMode.Polydirectional;

        [Header("Common Ability Settings")]
        [SerializeField] private EAbilityStateManagementMode m_abilityStateManagementMode = EAbilityStateManagementMode.Automatic;

        [Header("Effect Settings")]
        [SerializeReference, SubclassSelector] protected IEffect[] m_effects;

        public virtual void GenerateAdditionalDescriptionLines(List<AbilityDescriptionLine> lines)
        {
            GenerateDescriptionLinesFromEffects(lines, m_effects);
        }

        protected void GenerateDescriptionLinesFromEffects(List<AbilityDescriptionLine> lines, IEffect[] effects)
        {
            foreach (IEffect effect in effects)
            {
                EffectDescription desc = effect.GenerateDescription();

                lines.Add(new()
                {
                    header = desc.name,
                    content = desc.details
                });
            }
        }
        public Sprite icon => m_icon;
        public string description => m_description;
        public string displayName => DisplayNameUtils.GetNameOrDefault(this, m_displayName);
        public GameObject prefab => m_prefab;
        public EAbilityStateManagementMode abilityStateManagementMode => m_abilityStateManagementMode;
        public EAbilityOrientationMode orientationMode => m_orientationMode;
        public IEnumerable<IEffect> effects => m_effects;

    }
}
