using System;
using UnityEngine;
using azixMcAze.SerializableDictionary;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class GameFlagTaskProgressDataBlock : QuestTaskProgressDataBlock
    {
        public override IQuestTaskProgress CreateInstance() => new GameFlagTaskProgress(this);
    }

    public class GameFlagTaskProgress : QuestTaskProgress<GameFlagTaskProgressDataBlock>
    {
        private GameFlagTask m_gameFlagTask => (GameFlagTask)m_task;

        public GameFlagTaskProgress(GameFlagTask task) : base(task) { }

        public GameFlagTaskProgress(GameFlagTaskProgressDataBlock block) : base(block) { }

        public override void OnProgressTrackingStarted()
        {
            GameManager.NotificationSystem.gameFlagChanged.AddListener(OnGameFlagChanged);
        }

        public override void OnProgressTrackingStopped()
        {
            GameManager.NotificationSystem.gameFlagChanged.RemoveListener(OnGameFlagChanged);
        }

        public int CountCompleted()
        {
            int count = 0;

            foreach (var flag in m_gameFlagTask.gameFlags)
            {
                if (GameManager.GameFlagSystem.Get(flag.Key) == flag.Value)
                {
                    count++;
                }
            }

            return count;
        }

        public override bool IsCompleted()
        {
            return CountCompleted() == m_gameFlagTask.gameFlags.Count;
        }

        private void OnGameFlagChanged(string flag, bool state) => UpdateProgression();
    }

    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Quests_Tasks + nameof(GameFlagTask))]
    public class GameFlagTask : QuestTask
    {
        public SerializableDictionary<string, bool> gameFlags = new();

        public GameFlagTask()
        {
            m_title = "{0}/{1} conditions are met";
        }

        public override IQuestTaskProgress CreateTaskProgress() => new GameFlagTaskProgress(this);

        public override string GetCompletedTitle()
        {
            return StringFormatter.Format(m_title, gameFlags.Count, gameFlags.Count);
        }

        public override string GetInProgressTitle(IQuestTaskProgress progress)
        {
            return StringFormatter.Format(m_title, (progress as GameFlagTaskProgress).CountCompleted(), gameFlags.Count);
        }
    }
}
