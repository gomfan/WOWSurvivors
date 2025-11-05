using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class CompleteTask : ICommand
    {
        [SerializeField] private QuestTask m_task = null;

        public Task Execute()
        {
            foreach (QuestProgress progress in GameManager.JournalSystem.activeQuests)
            {
                progress.CompleteTask(m_task);
            }

            return Task.CompletedTask;
        }
    }
}
