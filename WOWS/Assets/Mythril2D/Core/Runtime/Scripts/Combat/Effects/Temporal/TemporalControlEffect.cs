using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public enum EControlType
    {
        Stun,
        Silence,
        Root,
    }

    [Serializable]
    public class TemporalControlEffect : ATemporalEffect
    {
        [Serializable]
        public struct ControlData
        {
            public EControlType controlType;
            [HideInInspector] public string unlockKey;
        }

        [SerializeField] private ControlData m_controlData;

        public override TermDefinition? info => GameManager.Config.GetTermDefinition(m_controlData.controlType);

        public override EEffectType GetEffectType() => EEffectType.Debuff;

        protected EActionFlags GetActionFlags()
        {
            switch (m_controlData.controlType)
            {
                case EControlType.Stun: return EActionFlags.Move | EActionFlags.UseAbility | EActionFlags.UpdateTargetDirection;
                case EControlType.Silence: return EActionFlags.UseAbility;
                case EControlType.Root: return EActionFlags.Move;
            }

            return EActionFlags.None;
        }

        protected override bool OnApply()
        {
            // Use a handle system to lock actions
            // Calling LockActions should return a handle/key to revert the lock
            m_controlData.unlockKey = m_effectData.target.instance.LockActions(GetActionFlags());
            return true;
        }

        protected override void OnCompleted()
        {
            m_effectData.target.instance.UnlockActions(m_controlData.unlockKey);
        }

        public override ITemporalEffect Clone()
        {
            return new TemporalControlEffect
            {
                m_effectData = m_effectData,
                m_temporalData = m_temporalData,
                m_controlData = m_controlData
            };
        }

        public override EffectDescription GenerateDescription()
        {
            var desc = base.GenerateDescription();
            desc.details = info.Value.description;
            return desc;
        }
    }
}
