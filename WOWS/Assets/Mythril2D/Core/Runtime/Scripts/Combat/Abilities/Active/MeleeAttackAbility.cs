using System.Collections.Generic;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class MeleeAttackAbility : ActiveAbility<ActiveAbilitySheet>
    {
        [Header("References")]
        [SerializeField] private Animator m_animator = null;
        [SerializeField] private Collider2D m_hitbox = null;
        [SerializeField] private EquipmentSpriteLibraryUpdater m_equipmentSpriteLibraryUpdater = null;

        [Header("Settings")]
        [SerializeField] private ContactFilter2D m_contactFilter;

        [Header("Animation Parameters")]
        [SerializeField] private string m_fireAnimationParameter = "fire";
        [SerializeField] private string m_cancelAnimationParameter = "cancel";

        public override void Init(CharacterBase character, AbilitySheet settings)
        {
            base.Init(character, settings);

            Debug.Assert(m_animator, ErrorMessages.InspectorMissingComponentReference<Animator>());
            Debug.Assert(m_animator.GetBehaviour<StateMessageDispatcher>(), string.Format("{0} not found on the melee attack animator controller", typeof(StateMessageDispatcher).Name));
        }

        protected override void Fire()
        {
            m_animator?.SetTrigger(m_fireAnimationParameter);
        }

        private void OnActionInterrupted()
        {
            if (abilitySheet.canInterupt && m_animator)
            {
                m_animator.SetTrigger(m_cancelAnimationParameter);
                OnMeleeAttackAnimationEnd();
            }
        }

        public void OnMeleeAttackAnimationStart()
        {
            m_equipmentSpriteLibraryUpdater?.UpdateVisual(m_character, EEquipmentType.Weapon);
        }

        public void OnMeleeAttackAnimationEnd()
        {
            if (!m_character.dead)
            {
                m_equipmentSpriteLibraryUpdater?.ResetVisual();
                TerminateCasting();
            }
        }

        public void ApplyHit()
        {
            if (!m_character.dead)
            {
                List<Collider2D> colliders = new();
                Physics2D.OverlapCollider(m_hitbox, m_contactFilter, colliders);

                List<CharacterBase> targets = new();

                foreach (Collider2D collider in colliders)
                {
                    if (collider.TryGetComponent(out CharacterBase character) && character != m_character)
                    {
                        targets.Add(character);
                    }
                }

                ApplyEffects(targets, activeAbilitySheet.effects);
            }
        }
    }
}
