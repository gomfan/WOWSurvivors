using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class InnInteraction : IInteraction
    {
        [Header("Dialogues")]
        [SerializeField] private DialogueSequence m_dialogueIfCanPay = null;
        [SerializeField] private DialogueSequence m_dialogueIfCannotPay = null;

        [Header("References")]
        [SerializeField] private Inn m_inn = null;

        public async Task<bool> TryExecute(CharacterBase source, IInteractionTarget target)
        {
            if (m_inn != null)
            {
                if (GameManager.InventorySystem.HasSufficientFunds(m_inn.price))
                {
                    await target.Say(m_dialogueIfCanPay, (messages) =>
                    {
                        if (messages.Contains(EDialogueMessageType.Accept))
                        {
                            GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_inn.healingSound);
                            GameManager.InventorySystem.RemoveMoney(m_inn.price);
                            GameManager.Player.Heal(m_inn.healAmount);
                            GameManager.Player.RecoverMana(m_inn.manaRecoveredAmount);
                        }
                    }, m_inn.price.ToString());
                }
                else
                {
                    await target.Say(m_dialogueIfCannotPay);
                }

                return true;
            }

            return false;
        }
    }
}
