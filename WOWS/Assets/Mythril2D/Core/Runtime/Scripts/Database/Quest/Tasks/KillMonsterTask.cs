using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class KillMonsterTaskProgressDataBlock : QuestTaskProgressDataBlock
    {
        public int monstersKilled;

        public override IQuestTaskProgress CreateInstance() => new KillMonsterTaskProgress(this);
    }

    public class KillMonsterTaskProgress : QuestTaskProgress<KillMonsterTaskProgressDataBlock>
    {
        public int monstersKilled { get; private set; } = 0;

        private KillMonsterTask m_killMonsterTask => (KillMonsterTask)m_task;

        public KillMonsterTaskProgress(KillMonsterTask task) : base(task) { }

        public KillMonsterTaskProgress(KillMonsterTaskProgressDataBlock block) : base(block) { }

        public override void OnProgressTrackingStarted()
        {
            GameManager.NotificationSystem.monsterKilled.AddListener(OnMonsterKilled);
        }

        public override void OnProgressTrackingStopped()
        {
            GameManager.NotificationSystem.monsterKilled.RemoveListener(OnMonsterKilled);
        }

        public override bool IsCompleted()
        {
            return monstersKilled >= m_killMonsterTask.monstersToKill;
        }

        private void OnMonsterKilled(MonsterSheet monster)
        {
            if (monster == m_killMonsterTask.monster)
            {
                ++monstersKilled;
                UpdateProgression();
            }
        }

        public override KillMonsterTaskProgressDataBlock CreateDataBlock()
        {
            KillMonsterTaskProgressDataBlock block = base.CreateDataBlock();
            block.monstersKilled = monstersKilled;
            return block;
        }

        public override void LoadDataBlock(KillMonsterTaskProgressDataBlock block)
        {
            base.LoadDataBlock(block);
            monstersKilled = block.monstersKilled;
        }
    }

    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Quests_Tasks + nameof(KillMonsterTask))]
    public class KillMonsterTask : QuestTask
    {
        public MonsterSheet monster = null;
        public int monstersToKill = 1;

        public KillMonsterTask()
        {
            m_title = "Kill {0} ({1}/{2})";
        }

        public override IQuestTaskProgress CreateTaskProgress() => new KillMonsterTaskProgress(this);

        public override string GetCompletedTitle()
        {
            return StringFormatter.Format(m_title, monster.displayName, monstersToKill, monstersToKill);
        }

        public override string GetInProgressTitle(IQuestTaskProgress progress)
        {
            return StringFormatter.Format(m_title, monster.displayName, ((KillMonsterTaskProgress)progress).monstersKilled, monstersToKill);
        }
    }
}
