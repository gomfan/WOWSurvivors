using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class DashAbility : ActiveAbility<DashAbilitySheet>
    {
        [Header("Reference")]
        [SerializeField] private ParticleSystem m_particleSystem = null;

        public override void Init(CharacterBase character, AbilitySheet settings)
        {
            base.Init(character, settings);
        }

        public override EAbilityFireCheckResult CanFire()
        {
            EAbilityFireCheckResult baseResult = base.CanFire();

            if (baseResult == EAbilityFireCheckResult.Valid && m_character.IsPushed())
            {
                return EAbilityFireCheckResult.Incapacitated;
            }

            return baseResult;
        }

        protected override void Fire()
        {
            Vector2 direction = m_character.GetTargetDirection();

            m_character.SetLookAtDirection(direction);
            m_character.Push(direction, activeAbilitySheet.dashStrength, activeAbilitySheet.dashResistance);
            m_particleSystem.Play();

            TerminateCasting();
        }
    }
}
