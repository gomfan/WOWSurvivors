using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class CraftInteraction : IInteraction
    {
        [Header("Dialogues")]
        [SerializeField] private DialogueSequence m_dialogue = null;

        [Header("References")]
        [SerializeField] private CraftingStation m_craftingStation = null;

        public async Task<bool> TryExecute(CharacterBase source, IInteractionTarget target)
        {
            if (m_craftingStation != null)
            {
                await target.Say(m_dialogue, async (messages) =>
                {
                    if (messages.Contains(EDialogueMessageType.Accept))
                    {
                        var result = new TaskCompletionSource<bool>();
                        GameManager.NotificationSystem.craftRequested.Invoke(m_craftingStation, result);
                        await result.Task;
                    }
                });

                return true;
            }

            return false;
        }
    }
}
