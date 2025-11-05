using System;
using System.Collections.Generic;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class AbilityBaseDataBlock : PersistableDataBlock
    {
        public DatabaseEntryReference<AbilitySheet> sheet;
    }

    public abstract class AbilityBase : Persistable
    {
        public abstract AbilitySheet baseAbilitySheet { get; }

        protected CharacterBase m_character = null;

        public virtual void Init(CharacterBase character, AbilitySheet settings)
        {
            m_character = character;
        }

        protected void ApplyEffectsToSelf(IEnumerable<IEffect> effects, EffectImpactSettings? effectImpactSettings = null)
        {
            ApplyEffects(new[] { m_character }, effects, effectImpactSettings);
        }

        protected void ApplyEffects(IEnumerable<CharacterBase> targets, IEnumerable<IEffect> effects, EffectImpactSettings? effectImpactSettings = null)
        {
            IEnumerable<CharacterBase> validTargets = FilterInvalidTargets(targets);
            EffectApplicationResult result = EffectDispatcher.Apply(m_character, validTargets, effects, effectImpactSettings);
            OnEffectsApplied(result);
        }

        protected virtual IEnumerable<CharacterBase> FilterInvalidTargets(IEnumerable<CharacterBase> targets) => targets;
        protected virtual void OnEffectsApplied(EffectApplicationResult result) { }
        public virtual void UpdateCooldowns() { }
        public virtual void Reset() { }
        public virtual void Interrupt() { }

        protected override Type GetDataBlockType() => typeof(AbilityBaseDataBlock);

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);

            if (baseAbilitySheet.abilityStateManagementMode == AbilitySheet.EAbilityStateManagementMode.Automatic)
            {
                // This make sure that an ability that was in progress when the game was saved is not active when the game is loaded.
                // Otherwise we would need to resume the exact animation state and other properties of the ability.
                // Do note that once the game is loaded, the ability will be on cooldown, even though it might not have applied its effect.
                // We could also force the ability cooldown to be 0 if we want to be more generous.
                block.state = EPersistableObjectState.Inactive;
            }

            // Note: we need to save the ability sheet for reference, the CharacterBase will need it to reinitialize the ability on load
            block.As<AbilityBaseDataBlock>().sheet = GameManager.Database.CreateReference(baseAbilitySheet);
        }

        protected override void OnLoad(PersistableDataBlock block)
        {
            base.OnLoad(block);

            // Note: we don't need to load the ability sheet from the block as it's already recovered in the Init method.
            AbilityBaseDataBlock abilityBlock = block.As<AbilityBaseDataBlock>();
        }
    }
}
