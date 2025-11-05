using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Abilities + nameof(ProjectileAbilitySheet))]
    public class ProjectileAbilitySheet : ActiveAbilitySheet
    {
        [Header("Projectile Ability Settings")]
        [SerializeField] private GameObject m_projectile;
        [SerializeField] private float m_projectileSpeed = 5.0f;
        [SerializeField] private int m_projectileCount = 1;
        [SerializeField] private float m_spread = 0.0f;

        [Header("Explosion Settings")]
        [SerializeField] private float m_explosionRadius = 0.0f;
        [SerializeField] private bool m_explosionApplyBaseEffects = true;
        [SerializeField] private bool m_explosionBaseEffectsIgnorePrimaryTarget = true;
        [SerializeField] private bool m_explosionAdditionalEffectsIgnorePrimaryTarget = false;
        [SerializeReference, SubclassSelector] private IEffect[] m_explosionAdditionalEffects;

        public GameObject projectile => m_projectile;
        public float projectileSpeed => m_projectileSpeed;
        public int projectileCount => m_projectileCount;
        public float spread => m_spread;
        public float explosionRadius => m_explosionRadius;
        public bool explosionApplyBaseEffects => m_explosionApplyBaseEffects;
        public bool explosionBaseEffectsIgnorePrimaryTarget => m_explosionBaseEffectsIgnorePrimaryTarget;
        public bool explosionAdditionalEffectsIgnorePrimaryTarget => m_explosionAdditionalEffectsIgnorePrimaryTarget;
        public IEffect[] explosionAdditionalEffects => m_explosionAdditionalEffects;
    }
}
