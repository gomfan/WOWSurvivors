namespace Gyvr.Mythril2D
{
    public class NPC : Character<NPCSheet>
    {
        protected virtual void Start()
        {
            GameManager.NotificationSystem.questUnlocked.AddListener(OnQuestStatusChanged);
            GameManager.NotificationSystem.questAvailabilityChanged.AddListener(OnQuestAvailabilityChanged);
            GameManager.NotificationSystem.questCompleted.AddListener(OnQuestStatusChanged);
            GameManager.NotificationSystem.questFullfilled.AddListener(OnQuestStatusChanged);
            GameManager.NotificationSystem.questProgressionUpdated.AddListener(OnQuestStatusChanged);
            GameManager.NotificationSystem.questStarted.AddListener(OnQuestStatusChanged);

            UpdateFloatingIcon();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            GameManager.NotificationSystem.questUnlocked.RemoveListener(OnQuestStatusChanged);
            GameManager.NotificationSystem.questAvailabilityChanged.AddListener(OnQuestAvailabilityChanged);
            GameManager.NotificationSystem.questCompleted.RemoveListener(OnQuestStatusChanged);
            GameManager.NotificationSystem.questFullfilled.RemoveListener(OnQuestStatusChanged);
            GameManager.NotificationSystem.questProgressionUpdated.RemoveListener(OnQuestStatusChanged);
            GameManager.NotificationSystem.questStarted.RemoveListener(OnQuestStatusChanged);
        }

        private void OnQuestStatusChanged(Quest quest) => UpdateFloatingIcon();
        private void OnQuestAvailabilityChanged(Quest quest, bool available) => UpdateFloatingIcon();

        private void UpdateFloatingIcon()
        {
            if (GameManager.JournalSystem.GetQuestToComplete(this) != null)
            {
                SetFloatingIcon(EFloatingIcon.QuestCompleted);
            }
            else if (GameManager.JournalSystem.GetTaskToComplete(this) != null)
            {
                SetFloatingIcon(EFloatingIcon.QuestTalkTo);
            }
            else if (GameManager.JournalSystem.GetQuestToStart(this) != null)
            {
                SetFloatingIcon(EFloatingIcon.QuestAvailable);
            }
            else if (GameManager.JournalSystem.GetNonFullfilledQuestToReportTo(this) != null)
            {
                SetFloatingIcon(EFloatingIcon.QuestInProgress);
            }
            else
            {
                SetFloatingIcon(EFloatingIcon.None);
            }
        }
    }
}
