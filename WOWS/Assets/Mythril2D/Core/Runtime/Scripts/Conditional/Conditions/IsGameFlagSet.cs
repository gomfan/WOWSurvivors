using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class IsGameFlagSet : ABaseCondition
    {
        [SerializeField] private string m_gameFlag = string.Empty;

        public override bool Evaluate() => GameManager.GameFlagSystem.Get(m_gameFlag);

        protected override void OnStartListening()
        {
            GameManager.NotificationSystem.gameFlagChanged.AddListener(OnGameFlagChanged);
        }

        protected override void OnStopListening()
        {
            GameManager.NotificationSystem.gameFlagChanged.RemoveListener(OnGameFlagChanged);
        }

        private void OnGameFlagChanged(string gameFlag, bool state) => NotifyStateChange();
    }
}
