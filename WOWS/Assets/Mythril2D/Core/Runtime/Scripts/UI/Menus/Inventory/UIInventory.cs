using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class UIInventory : AUIMenu
    {
        [SerializeField] private UIInventoryEquipment m_equipment = null;
        [SerializeField] private UIInventoryBag m_bag = null;
        [SerializeField] private UIInventoryStats m_stats = null;

        protected override void OnInit()
        {
            m_bag.Init();
        }

        protected override void OnMenuShown(params object[] args)
        {
            UpdateUI();
        }

        public override GameObject FindSomethingToSelect()
        {
            UINavigationCursorTarget bagNavigationTarget = m_bag.FindNavigationTarget();

            if (bagNavigationTarget && bagNavigationTarget.gameObject.activeInHierarchy)
            {
                return bagNavigationTarget.gameObject;
            }
            else
            {
                UINavigationCursorTarget equipmentNavigationTarget = m_equipment.FindNavigationTarget();

                if (equipmentNavigationTarget && equipmentNavigationTarget.isActiveAndEnabled)
                {
                    return equipmentNavigationTarget.gameObject;
                }
            }

            return null;
        }

        // Message called by children using SendMessageUpward when the bag or equipment changed
        private void UpdateUI()
        {
            m_bag.UpdateSlots();
            m_equipment.UpdateSlots();
            m_stats.UpdateUI();
        }

        private async void OnItemClicked(Item item, EItemLocation location)
        {
            await item.Use(GameManager.Player, location);
            UpdateUI();
        }

        private void OnBagItemClicked(Item item) => OnItemClicked(item, EItemLocation.Bag);
        private void OnEquipmentItemClicked(Item item) => OnItemClicked(item, EItemLocation.Equipment);
    }
}
