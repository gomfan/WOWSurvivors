using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public enum EQuestState
    {
        Unlocked,
        Available,
        Active,
        Fullfilled,
        Completed
    }

    [Serializable]
    public class IsQuestInState : ABaseCondition
    {
        [SerializeField] private Quest m_quest = null;
        [SerializeField] private EQuestState m_state = EQuestState.Unlocked;

        public override bool Evaluate()
        {
            switch (m_state)
            {
                case EQuestState.Unlocked: return GameManager.JournalSystem.IsQuestUnlocked(m_quest);
                case EQuestState.Available: return GameManager.JournalSystem.IsQuestAvailable(m_quest);
                case EQuestState.Active: return GameManager.JournalSystem.IsQuestActive(m_quest);
                case EQuestState.Fullfilled: return GameManager.JournalSystem.IsQuestFullfilled(m_quest);
                case EQuestState.Completed: return GameManager.JournalSystem.IsQuestCompleted(m_quest);
            }

            return false;
        }

        protected override void OnStartListening()
        {
            switch (m_state)
            {
                case EQuestState.Unlocked:
                    GameManager.NotificationSystem.questUnlocked.AddListener(OnQuestStateChanged);
                    GameManager.NotificationSystem.questStarted.AddListener(OnQuestStateChanged);
                    break;

                case EQuestState.Available:
                    GameManager.NotificationSystem.questUnlocked.AddListener(OnQuestStateChanged);
                    GameManager.NotificationSystem.levelUp.AddListener(OnLevelUp);
                    GameManager.NotificationSystem.questStarted.AddListener(OnQuestStateChanged);
                    break;

                case EQuestState.Active:
                    GameManager.NotificationSystem.questStarted.AddListener(OnQuestStateChanged);
                    GameManager.NotificationSystem.questFullfilled.AddListener(OnQuestStateChanged);
                    break;

                case EQuestState.Fullfilled:
                    GameManager.NotificationSystem.questFullfilled.AddListener(OnQuestStateChanged);
                    GameManager.NotificationSystem.questCompleted.AddListener(OnQuestStateChanged);
                    break;

                case EQuestState.Completed:
                    GameManager.NotificationSystem.questCompleted.AddListener(OnQuestStateChanged);
                    break;
            }
        }

        protected override void OnStopListening()
        {
            switch (m_state)
            {
                case EQuestState.Unlocked:
                    GameManager.NotificationSystem.questUnlocked.RemoveListener(OnQuestStateChanged);
                    GameManager.NotificationSystem.questStarted.RemoveListener(OnQuestStateChanged);
                    break;

                case EQuestState.Available:
                    GameManager.NotificationSystem.questUnlocked.RemoveListener(OnQuestStateChanged);
                    GameManager.NotificationSystem.levelUp.RemoveListener(OnLevelUp);
                    GameManager.NotificationSystem.questStarted.RemoveListener(OnQuestStateChanged);
                    break;

                case EQuestState.Active:
                    GameManager.NotificationSystem.questStarted.RemoveListener(OnQuestStateChanged);
                    GameManager.NotificationSystem.questFullfilled.RemoveListener(OnQuestStateChanged);
                    break;

                case EQuestState.Fullfilled:
                    GameManager.NotificationSystem.questFullfilled.RemoveListener(OnQuestStateChanged);
                    GameManager.NotificationSystem.questCompleted.RemoveListener(OnQuestStateChanged);
                    break;

                case EQuestState.Completed:
                    GameManager.NotificationSystem.questCompleted.RemoveListener(OnQuestStateChanged);
                    break;
            }
        }

        private void OnQuestStateChanged(Quest quest) => NotifyStateChange();
        private void OnLevelUp(int level) => NotifyStateChange();
    }
}
