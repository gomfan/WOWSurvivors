using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Gyvr.Mythril2D
{
    public enum EAbilityFireCheckResult
    {
        Valid,
        OnCooldown,
        NotEnoughMana,
        Incapacitated,
        Unknown
    }

    public interface ITriggerableAbility
    {
        public void Fire(UnityAction onAbilityEnded);
        public EAbilityFireCheckResult CanFire();
        public ActiveAbilitySheet GetSheet();
        public ActiveAbilityBase GetAbilityBase();
    }

    [Serializable]
    public class ActiveAbilityBaseDataBlock : AbilityBaseDataBlock
    {
        public float remainingCooldownTimer;
    }

    public abstract class ActiveAbilityBase : Ability<ActiveAbilitySheet>, ITriggerableAbility
    {
        private UnityAction m_onAbilityEndedCallback = null;

        public float remainingCooldown => m_remainingCooldownTimer;
        public float cooldown => abilitySheet.cooldown;

        protected float m_remainingCooldownTimer = 0.0f;

        protected abstract void Fire();

        public ActiveAbilityBase GetAbilityBase() => this;
        public ActiveAbilitySheet GetSheet() => abilitySheet;

        public void Fire(UnityAction onAbilityEnded)
        {
            m_onAbilityEndedCallback = onAbilityEnded;
            m_character.DisableActions(abilitySheet.disabledActionsWhileCasting);
            Fire();
            ApplyEffects(new[] { m_character }, abilitySheet.autoAppliedEffectsToCasterOnFire);
            ConsumeMana();
            StartCooldown();
        }

        public override void Reset()
        {
            base.Reset();
            m_remainingCooldownTimer = 0.0f;
        }

        public override void Interrupt()
        {
            base.Interrupt();
            TerminateCasting();
        }

        protected void StartCooldown()
        {
            m_remainingCooldownTimer = abilitySheet.cooldown;
        }

        public override void UpdateCooldowns()
        {
            base.UpdateCooldowns();
            m_remainingCooldownTimer = math.max(0.0f, m_remainingCooldownTimer - Time.deltaTime);
        }

        public virtual EAbilityFireCheckResult CanFire()
        {
            if (m_remainingCooldownTimer > 0.0f)
            {
                return EAbilityFireCheckResult.OnCooldown;
            }

            if (m_character.currentStats[EStat.Mana] < abilitySheet.manaCost)
            {
                return EAbilityFireCheckResult.NotEnoughMana;
            }

            if (!m_character.Can(EActionFlags.UseAbility))
            {
                return EAbilityFireCheckResult.Incapacitated;
            }

            return EAbilityFireCheckResult.Valid;
        }

        protected virtual void ConsumeMana()
        {
            m_character.ConsumeMana(abilitySheet.manaCost);
        }

        protected void TerminateCasting()
        {
            m_character.EnableActions(abilitySheet.disabledActionsWhileCasting);
            m_onAbilityEndedCallback?.Invoke();
            m_onAbilityEndedCallback = null;
        }

        protected override Type GetDataBlockType() => typeof(ActiveAbilityBaseDataBlock);

        protected override void OnLoad(PersistableDataBlock block)
        {
            base.OnLoad(block);
            m_remainingCooldownTimer = block.As<ActiveAbilityBaseDataBlock>().remainingCooldownTimer;
        }

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);
            block.As<ActiveAbilityBaseDataBlock>().remainingCooldownTimer = m_remainingCooldownTimer;
        }
    }
}
