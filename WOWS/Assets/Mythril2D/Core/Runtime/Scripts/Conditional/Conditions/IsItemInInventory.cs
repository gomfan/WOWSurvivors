using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class IsItemInInventory : ABaseCondition
    {
        [SerializeField] private Item m_item = null;

        public override bool Evaluate() => GameManager.InventorySystem.HasItemInBag(m_item);

        protected override void OnStartListening()
        {
            GameManager.NotificationSystem.itemAdded.AddListener(OnItemAdded);
            GameManager.NotificationSystem.itemRemoved.AddListener(OnItemRemoved);
        }

        protected override void OnStopListening()
        {
            GameManager.NotificationSystem.itemAdded.RemoveListener(OnItemAdded);
            GameManager.NotificationSystem.itemRemoved.RemoveListener(OnItemRemoved);
        }

        private void OnItemAdded(Item item, int quantity, EItemTransferType source) => NotifyStateChange();
        private void OnItemRemoved(Item item, int quantity, EItemTransferType source) => NotifyStateChange();
    }
}
