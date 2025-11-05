using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ContactDamageAbilityDataBlock : PassiveAbilityBaseDataBlock
    {
        public PerTargetCooldownDataBlock<CharacterBase> perTargetCooldowns;
    }

    public class ContactDamageAbility : PassiveAbility<ContactDamageAbilitySheet>
    {
        [Header("Settings")]
        [SerializeField] private ContactFilter2D m_contactFilter2D;

        private Collider2D m_hitbox = null;

        public override void Init(CharacterBase character, AbilitySheet settings)
        {
            base.Init(character, settings);

            // Use the character collider as the contact damage hitbox
            m_hitbox = character.GetComponent<Collider2D>();
        }

        private void Awake()
        {
            m_hitbox = GetComponentInParent<Collider2D>();
        }

        private void FixedUpdate()
        {
            List<Collider2D> colliders = new();
            Physics2D.OverlapCollider(m_hitbox, m_contactFilter2D, colliders);

            List<CharacterBase> targets = new();

            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent(out CharacterBase character))
                {
                    targets.Add(character);
                }
            }

            ApplyEffects(targets, abilitySheet.effects);
        }

        protected override IEnumerable<CharacterBase> FilterInvalidTargets(IEnumerable<CharacterBase> targets)
        {
            return m_perTargetCooldowns.FilterInvalidTargets(base.FilterInvalidTargets(targets));
        }

        protected PerTargetCooldown<CharacterBase> m_perTargetCooldowns = new();

        public override void UpdateCooldowns()
        {
            base.UpdateCooldowns();
            m_perTargetCooldowns.Update();
        }

        protected override void OnEffectsApplied(EffectApplicationResult result)
        {
            base.OnEffectsApplied(result);

            foreach (var target in result.affectedTargets)
            {
                m_perTargetCooldowns.StartCooldown(target, passiveAbilitySheet.perTargetCooldown);
            }
        }

        protected override Type GetDataBlockType() => typeof(ContactDamageAbilityDataBlock);

        protected override void OnLoad(PersistableDataBlock block)
        {
            base.OnLoad(block);
            m_perTargetCooldowns.LoadDataBlock(block.As<ContactDamageAbilityDataBlock>().perTargetCooldowns);
        }

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);
            block.As<ContactDamageAbilityDataBlock>().perTargetCooldowns = m_perTargetCooldowns.CreateDataBlock();
        }
    }
}
