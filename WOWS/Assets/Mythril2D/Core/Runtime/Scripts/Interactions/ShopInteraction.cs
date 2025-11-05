using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ShopInteraction : IInteraction
    {
        [Header("Dialogues")]
        [SerializeField] private DialogueSequence m_dialogue = null;

        [Header("References")]
        [SerializeField] private Shop m_shop = null;

        public async Task<bool> TryExecute(CharacterBase source, IInteractionTarget target)
        {
            if (m_shop != null)
            {
                await target.Say(m_dialogue, async (messages) =>
                {
                    if (messages.Contains(EDialogueMessageType.Accept))
                    {
                        var onMenuClosed = new TaskCompletionSource<bool>();
                        GameManager.NotificationSystem.shopRequested.Invoke(m_shop, onMenuClosed);
                        await onMenuClosed.Task;
                    }
                });

                return true;
            }

            return false;
        }
    }
}
