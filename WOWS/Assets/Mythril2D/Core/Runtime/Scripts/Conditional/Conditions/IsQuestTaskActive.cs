using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class IsQuestTaskActive : ABaseCondition
    {
        [SerializeField] private QuestTask m_task = null;

        public override bool Evaluate() => GameManager.JournalSystem.IsTaskActive(m_task);

        protected override void OnStartListening()
        {
            GameManager.NotificationSystem.questStarted.AddListener(OnQuestStarted);
            GameManager.NotificationSystem.questProgressionUpdated.AddListener(OnQuestProgressionUpdated);
        }

        protected override void OnStopListening()
        {
            GameManager.NotificationSystem.questStarted.RemoveListener(OnQuestStarted);
            GameManager.NotificationSystem.questProgressionUpdated.RemoveListener(OnQuestProgressionUpdated);
        }

        private void OnQuestStarted(Quest quest) => NotifyStateChange();
        private void OnQuestProgressionUpdated(Quest quest) => NotifyStateChange();
    }
}
