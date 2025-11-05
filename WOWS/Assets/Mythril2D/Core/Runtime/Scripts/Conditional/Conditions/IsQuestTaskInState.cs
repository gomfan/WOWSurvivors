using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Flags]
    public enum EQuestTaskStateFlags
    {
        [HideInInspector] None = 0,
        InProgress = 1 << 0,
        Completed = 1 << 1,
        [HideInInspector] All = ~None
    }

    [Serializable]
    public class IsQuestTaskInState : ABaseCondition
    {
        [SerializeField] private QuestTask m_task = null;
        [SerializeField] private EQuestTaskStateFlags m_stateFlags = EQuestTaskStateFlags.None;

        public override bool Evaluate()
        {
            return
                (m_stateFlags.HasFlag(EQuestTaskStateFlags.InProgress) && GameManager.JournalSystem.IsTaskActive(m_task)) ||
                (m_stateFlags.HasFlag(EQuestTaskStateFlags.Completed) && GameManager.JournalSystem.IsTaskCompleted(m_task));
        }

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
