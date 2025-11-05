using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class QuestInteraction : IInteraction
    {
        private async Task<bool> TryProgressingQuest(NPC npc)
        {
            TalkToNPCTaskProgress taskProgress = GameManager.JournalSystem.GetTaskToComplete(npc);
            if (taskProgress != null)
            {
                await npc.Say(taskProgress.talkToNPCTask.dialogue);
                taskProgress.MarkAsCompleted();
                return true;
            }

            return false;
        }

        private async Task<bool> TryCompletingQuest(NPC npc)
        {
            Quest quest = GameManager.JournalSystem.GetQuestToComplete(npc);

            if (quest)
            {
                if (quest.questCompletedDialogue != null)
                {
                    await npc.Say(quest.questCompletedDialogue, (actionFeed) =>
                    {
                        GameManager.JournalSystem.CompleteQuest(quest);
                    });

                    return true;
                }
                else
                {
                    Debug.LogErrorFormat("No quest completed dialogue provided for [{0}]", quest.title);
                }
            }

            return false;
        }

        private DialogueSequence FindQuestHintDialogue(Quest quest)
        {
            // Look for quest hint overrides (some tasks may have specific hints)
            if (quest.questHintDialogueOverrides.Count > 0)
            {
                foreach (var activeQuest in GameManager.JournalSystem.activeQuests)
                {
                    foreach (var task in activeQuest.currentTasks)
                    {
                        if (quest.questHintDialogueOverrides.ContainsKey(task.task))
                        {
                            return quest.questHintDialogueOverrides[task.task];
                        }
                    }
                }
            }

            if (quest.questHintDialogue != null)
            {
                return quest.questHintDialogue;
            }

            return null;
        }

        private async Task<bool> TryGivingHint(NPC npc)
        {
            // Try to find a hint for a fullfilled quest (quest with no task, such as "Talk to X")
            Quest quest = GameManager.JournalSystem.GetFullfilledQuest(npc);

            if (!quest)
            {
                // Try to find a hint for a started quest
                quest = GameManager.JournalSystem.GetStartedQuest(npc);
            }

            if (quest != null)
            {
                DialogueSequence dialogue = FindQuestHintDialogue(quest);

                if (dialogue)
                {
                    await npc.Say(dialogue);
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> TryOfferingQuest(NPC npc)
        {
            Quest quest = GameManager.JournalSystem.GetQuestToStart(npc);

            if (quest)
            {
                if (quest.questOfferDialogue != null)
                {
                    await npc.Say(quest.questOfferDialogue, (messages) =>
                    {
                        if (messages.Contains(EDialogueMessageType.Accept))
                        {
                            GameManager.JournalSystem.StartQuest(quest);
                        }
                    });

                    return true;
                }
                else
                {
                    Debug.LogErrorFormat("No quest offer dialogue provided for [{0}]", quest.title);
                }
            }

            return false;
        }

        public async Task<bool> TryExecute(CharacterBase source, IInteractionTarget target)
        {
            if (target is NPC npc)
            {
                if (!await TryCompletingQuest(npc))
                {
                    if (!await TryProgressingQuest(npc))
                    {
                        if (!await TryOfferingQuest(npc))
                        {
                            if (!await TryGivingHint(npc))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("QuestInteraction can only be used with NPC targets.");
                return false;
            }

            return true;
        }
    }
}
