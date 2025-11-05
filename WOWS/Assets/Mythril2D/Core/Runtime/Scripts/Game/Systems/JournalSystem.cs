using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class JournalDataBlock : DataBlock
    {
        public DatabaseEntryReference<Quest>[] unlockedQuests;
        public QuestProgressDataBlock[] activeQuests;
        public DatabaseEntryReference<Quest>[] fullfilledQuests;
        public DatabaseEntryReference<Quest>[] completedQuests;
    }

    public class JournalSystem : AGameSystem, IDataBlockHandler<JournalDataBlock>
    {
        [Header("Audio")]
        [SerializeField] private AudioClipResolver m_questStartedSound;
        [SerializeField] private AudioClipResolver m_questCompletedSound;

        public List<Quest> unlockedQuests => m_unlockedQuests;
        public List<Quest> availableQuests => m_availableQuests;
        public List<QuestProgress> activeQuests => m_activeQuests;
        public List<Quest> fullfilledQuests => m_fullfilledQuests;
        public List<Quest> completedQuests => m_completedQuests;

        private List<Quest> m_unlockedQuests = new();
        private List<Quest> m_availableQuests = new();
        private List<QuestProgress> m_activeQuests = new();
        private List<Quest> m_fullfilledQuests = new();
        private List<Quest> m_completedQuests = new();

        public bool IsQuestUnlocked(Quest quest) => m_unlockedQuests.Contains(quest);
        public bool IsQuestAvailable(Quest quest) => m_availableQuests.Contains(quest);
        public bool IsQuestActive(Quest quest) => m_activeQuests.Find((QuestProgress progress) => progress.quest == quest) != null;
        public bool IsQuestFullfilled(Quest quest) => m_fullfilledQuests.Contains(quest);
        public bool IsQuestCompleted(Quest quest) => m_completedQuests.Contains(quest);

        public bool IsTaskActive(QuestTask task)
        {
            foreach (QuestProgress progress in activeQuests)
            {
                if (progress.currentTasks.Find((taskProgress) => taskProgress.task == task) != null)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsTaskCompleted(QuestTask task)
        {
            foreach (QuestProgress progress in activeQuests)
            {
                if (progress.completedTasks.Find((taskProgress) => taskProgress.task == task) != null)
                {
                    return true;
                }
            }

            foreach (Quest quest in m_fullfilledQuests.Union(m_completedQuests))
            {
                if (quest.tasks.Contains(task))
                {
                    return true;
                }
            }

            return false;
        }

        public void StartQuest(Quest quest)
        {
            QuestProgress instance = new(quest, OnQuestFullfilled);
            m_unlockedQuests.Remove(quest);
            m_availableQuests.Remove(quest);
            activeQuests.Add(instance);
            instance.Initialize();
            GameManager.NotificationSystem.questStarted.Invoke(quest);
            GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_questStartedSound);
        }

        public void OnQuestFullfilled(QuestProgress instance)
        {
            fullfilledQuests.Add(instance.quest);
            activeQuests.Remove(instance);
            GameManager.NotificationSystem.questFullfilled.Invoke(instance.quest);
        }

        public void CompleteQuest(Quest quest)
        {
            fullfilledQuests.Remove(quest);
            completedQuests.Add(quest);

            GameManager.NotificationSystem.questCompleted.Invoke(quest);
            GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_questCompletedSound);

            if (quest.repeatable)
            {
                UnlockQuest(quest);
            }

            quest.toExecuteOnQuestCompletion?.Execute();
        }

        public void UnlockQuest(Quest quest)
        {
            m_unlockedQuests.Add(quest);

            if (!m_availableQuests.Contains(quest) && CheckIfQuestRequirementsAreMet(quest))
            {
                m_availableQuests.Add(quest);
                GameManager.NotificationSystem.questAvailabilityChanged.Invoke(quest, true);
            }

            GameManager.NotificationSystem.questUnlocked.Invoke(quest);
        }

        public QuestProgress GetNonFullfilledQuestToReportTo(NPC npc)
        {
            return activeQuests.Find((quest) => quest.quest.reportTo == npc.characterSheet);
        }

        public Quest GetQuestToComplete(NPC npc)
        {
            return fullfilledQuests.Find((quest) => quest.reportTo == npc.characterSheet);
        }

        public TalkToNPCTaskProgress GetTaskToComplete(NPC npc)
        {
            foreach (var quest in m_activeQuests)
            {
                foreach (var task in quest.currentTasks)
                {
                    if (task is TalkToNPCTaskProgress progress && progress.talkToNPCTask.target == npc.characterSheet)
                    {
                        return progress;
                    }
                }
            }

            return null;
        }

        public Quest GetQuestToStart(NPC npc)
        {
            return availableQuests.Find((quest) => quest.offeredBy == npc.characterSheet);
        }

        public Quest GetStartedQuest(NPC npc)
        {
            List<QuestProgress> results = activeQuests.FindAll((quest) => quest.quest.offeredBy == npc.characterSheet);
            return results != null && results.Count > 0 ? results[0].quest : null;
        }

        public Quest GetFullfilledQuest(NPC npc)
        {
            List<Quest> results = fullfilledQuests.FindAll((quest) => quest.offeredBy == npc.characterSheet);
            return results != null && results.Count > 0 ? results[0] : null;
        }


        private void UpdateQuestsAvailability()
        {
            foreach (Quest quest in m_unlockedQuests)
            {
                if (!m_availableQuests.Contains(quest) && CheckIfQuestRequirementsAreMet(quest))
                {
                    m_availableQuests.Add(quest);
                    GameManager.NotificationSystem.questAvailabilityChanged.Invoke(quest, true);
                }
                else if (m_availableQuests.Contains(quest) && !CheckIfQuestRequirementsAreMet(quest))
                {
                    m_availableQuests.Remove(quest);
                    GameManager.NotificationSystem.questAvailabilityChanged.Invoke(quest, false);
                }
            }
        }

        private bool CheckIfQuestRequirementsAreMet(Quest quest)
        {
            return quest.requiredLevel <= GameManager.Player.level;
        }

        private void Start()
        {
            GameManager.NotificationSystem.levelUp.AddListener((int level) => UpdateQuestsAvailability());
        }

        public void LoadDataBlock(JournalDataBlock block)
        {
            m_unlockedQuests = block.unlockedQuests.Select(quest => GameManager.Database.LoadFromReference(quest)).ToList();
            m_fullfilledQuests = block.fullfilledQuests.Select(quest => GameManager.Database.LoadFromReference(quest)).ToList();
            m_completedQuests = block.completedQuests.Select(quest => GameManager.Database.LoadFromReference(quest)).ToList();

            m_activeQuests = new List<QuestProgress>(block.activeQuests.Length);

            foreach (QuestProgressDataBlock progressDataBlock in block.activeQuests)
            {
                QuestProgress progress = new(progressDataBlock, OnQuestFullfilled);
                m_activeQuests.Add(progress);
                progress.CheckFullfillment();
            }

            UpdateQuestsAvailability();
        }

        public JournalDataBlock CreateDataBlock()
        {
            return new JournalDataBlock
            {
                unlockedQuests = m_unlockedQuests.Select(quest => GameManager.Database.CreateReference(quest)).ToArray(),
                activeQuests = m_activeQuests.Select(qi => qi.CreateDataBlock()).ToArray(),
                fullfilledQuests = m_fullfilledQuests.Select(quest => GameManager.Database.CreateReference(quest)).ToArray(),
                completedQuests = m_completedQuests.Select(quest => GameManager.Database.CreateReference(quest)).ToArray()
            };
        }
    }
}