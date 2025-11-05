using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class IsAbilityUnlocked : ABaseCondition
    {
        [SerializeField] private ActiveAbilitySheet m_ability = null;

        public override bool Evaluate() => GameManager.Player.HasAbility(m_ability);

        protected override void OnStartListening()
        {
            GameManager.NotificationSystem.abilityAdded.AddListener(OnAbilityAdded);
            GameManager.NotificationSystem.abilityRemoved.AddListener(OnAbilityRemoved);
        }

        protected override void OnStopListening()
        {
            GameManager.NotificationSystem.abilityAdded.RemoveListener(OnAbilityAdded);
            GameManager.NotificationSystem.abilityRemoved.RemoveListener(OnAbilityRemoved);
        }

        private void OnAbilityAdded(AbilitySheet ability) => NotifyStateChange();
        private void OnAbilityRemoved(AbilitySheet ability) => NotifyStateChange();
    }
}
