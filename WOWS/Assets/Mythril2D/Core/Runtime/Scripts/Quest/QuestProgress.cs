using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class QuestProgressDataBlock : DataBlock
    {
        public DatabaseEntryReference<Quest> quest;
        [SerializeReference] public QuestTaskProgressDataBlock[] completedTasks;
        [SerializeReference] public QuestTaskProgressDataBlock[] currentTasks;
        public DatabaseEntryReference<QuestTask>[] nextTasks;
    }

    public class QuestProgress : IDataBlockHandler<QuestProgressDataBlock>
    {
        // Public Getters
        public List<IQuestTaskProgress> completedTasks => m_completedTasks;
        public List<IQuestTaskProgress> currentTasks => m_currentTasks;
        public Quest quest => m_quest;

        // Private Members
        private Quest m_quest = null;
        private List<IQuestTaskProgress> m_completedTasks = new();
        private List<IQuestTaskProgress> m_currentTasks = new();
        private Queue<QuestTask> m_nextTasks = new();
        private Action<QuestProgress> m_fullfilledCallback;

        public QuestProgress(Quest quest, Action<QuestProgress> fullfilledCallback)
        {
            m_quest = quest;
            m_fullfilledCallback = fullfilledCallback;
        }

        public QuestProgress(QuestProgressDataBlock block, Action<QuestProgress> fullfilledCallback)
        {
            m_fullfilledCallback = fullfilledCallback;
            LoadDataBlock(block);
        }

        public void Initialize()
        {
            foreach (QuestTask task in quest.tasks)
            {
                m_nextTasks.Enqueue(task);
            }

            // Handle the case where the quest has no tasks
            CheckFullfillment();

            UpdateCurrentTasks();
        }

        public void LoadDataBlock(QuestProgressDataBlock block)
        {
            m_quest = GameManager.Database.LoadFromReference(block.quest);
            m_completedTasks = block.completedTasks.Select(block => block.CreateInstance()).ToList();
            m_currentTasks = block.currentTasks.Select(block => block.CreateInstance()).ToList();
            m_nextTasks = new Queue<QuestTask>(block.nextTasks.Select(task => GameManager.Database.LoadFromReference(task)));

            foreach (IQuestTaskProgress task in m_currentTasks)
            {
                task.Initialize(OnTaskCompleted);
            }
        }

        public QuestProgressDataBlock CreateDataBlock()
        {
            return new QuestProgressDataBlock
            {
                quest = GameManager.Database.CreateReference(m_quest),
                completedTasks = m_completedTasks.Select(progress => progress.CreateDataBlock()).ToArray(),
                currentTasks = m_currentTasks.Select(progress => progress.CreateDataBlock()).ToArray(),
                nextTasks = m_nextTasks.Select(task => GameManager.Database.CreateReference(task)).ToArray()
            };
        }

        public void CheckFullfillment()
        {
            if (m_currentTasks.Count == 0 && m_nextTasks.Count == 0)
            {
                m_fullfilledCallback(this);
            }
        }

        public void UpdateCurrentTasks()
        {
            while (m_nextTasks.Count > 0)
            {
                QuestTask task = m_nextTasks.Dequeue();
                IQuestTaskProgress taskProgress = task.CreateTaskProgress();
                m_currentTasks.Add(taskProgress);
                taskProgress.Initialize(OnTaskCompleted);

                // If the next task requires previous tasks to be completed to become available, stop dequeuing tasks
                if (m_nextTasks.Count > 0 && m_nextTasks.Peek().requirePreviousTaskCompletion)
                {
                    return;
                }
            }
        }

        private void OnTaskCompleted(IQuestTaskProgress taskProgress)
        {
            m_currentTasks.Remove(taskProgress);
            m_completedTasks.Add(taskProgress);

            if (m_currentTasks.Count == 0)
            {
                if (m_nextTasks.Count > 0)
                {
                    UpdateCurrentTasks();
                }
                else
                {
                    m_fullfilledCallback(this);
                }
            }

            GameManager.NotificationSystem.questProgressionUpdated.Invoke(quest);
        }

        public void CompleteTask(QuestTask task)
        {
            foreach (IQuestTaskProgress taskProgress in m_currentTasks)
            {
                if (taskProgress.task == task)
                {
                    OnTaskCompleted(taskProgress);
                }
            }
        }
    }
}
