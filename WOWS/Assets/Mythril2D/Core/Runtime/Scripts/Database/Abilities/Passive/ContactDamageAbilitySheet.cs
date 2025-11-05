using UnityEngine;

namespace Gyvr.Mythril2D
{
    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Abilities + nameof(ContactDamageAbilitySheet))]
    public class ContactDamageAbilitySheet : PassiveAbilitySheet
    {
        [SerializeField] private float m_perTargetCooldown = 0.0f;

        public float perTargetCooldown => m_perTargetCooldown;
    }
}
