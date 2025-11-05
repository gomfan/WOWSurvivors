using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public struct ItemUsageResult
    {
        public bool success;
        public string message;
    }

    public abstract class AItemEffect : IItemEffect
    {
        [SerializeField] private bool m_consumeAfterUse = false;
        [SerializeField] private AudioClipResolver m_useAudio = null;

        protected abstract ItemUsageResult OnUse(Item item, CharacterBase target, EItemLocation location);

        public async Task<bool> TryUse(Item item, CharacterBase target, EItemLocation location)
        {
            ItemUsageResult result = OnUse(item, target, location);

            if (result.success)
            {
                GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_useAudio);

                await GameManager.DialogueSystem.Main.PlayNow(
                    string.IsNullOrEmpty(result.message) ?
                    $"You used {item.name}." :
                    result.message
                );

                if (m_consumeAfterUse)
                {
                    GameManager.InventorySystem.RemoveFromBag(item, 1, EItemTransferType.Use);
                }

                return true;
            }

            return false;
        }
    }
}
