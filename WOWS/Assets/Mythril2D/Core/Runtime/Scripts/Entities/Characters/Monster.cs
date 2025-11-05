namespace Gyvr.Mythril2D
{
    public class Monster : Character<MonsterSheet>
    {
        protected override void InitializeStats()
        {
            UpdateStats();
        }

        public void SetLevel(int level)
        {
            m_level = level;
            UpdateStats();
        }

        public override void LevelUp(bool silentMode = false)
        {
            base.LevelUp(silentMode);
            UpdateStats();
        }

        public void UpdateStats()
        {
            m_stats.Set(m_sheet.stats[m_level]);
        }

        public override void Kill()
        {
            base.Kill();

            if (!m_isSummoned)
            {
                GameManager.NotificationSystem.monsterKilled.Invoke(m_sheet);

                foreach (Loot loot in m_sheet.potentialLoot)
                {
                    if (GameManager.Player.level >= loot.minimumPlayerLevel && m_level >= loot.minimumMonsterLevel && loot.IsAvailable() && loot.ResolveDrop())
                    {
                        GameManager.InventorySystem.AddToBag(loot.item, loot.quantity, EItemTransferType.MonsterDrop);
                    }
                }

                GameManager.Player.AddExperience(m_sheet.experience[m_level]);
                GameManager.InventorySystem.AddMoney(m_sheet.money[m_level]);

                m_sheet.executeOnDeath?.Execute();
            }
        }

        protected override void OnStuckInAWall()
        {
            // If the monster is stuck in a wall, kill it.
            Kill();
        }
    }
}
