using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ItemStartQuestEffect : AItemEffect
    {
        [SerializeField] private string m_dialogueLine;
        [SerializeField] private Quest m_questToStart;

        protected override ItemUsageResult OnUse(Item item, CharacterBase target, EItemLocation location)
        {
            bool canPlayQuest =
                !GameManager.JournalSystem.IsQuestActive(m_questToStart) &&
                !GameManager.JournalSystem.IsQuestFullfilled(m_questToStart) &&
                (!GameManager.JournalSystem.IsQuestCompleted(m_questToStart) || m_questToStart.repeatable);

            if (canPlayQuest)
            {
                GameManager.JournalSystem.StartQuest(m_questToStart);

                return new()
                {
                    success = true,
                    message = m_dialogueLine
                };
            }

            return new() { success = false };
        }
    }
}
