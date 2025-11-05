using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using azixMcAze.SerializableDictionary;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class InventoryDataBlock : DataBlock
    {
        public int money;
        public SerializableDictionary<DatabaseEntryReference<Item>, int> items;
    }

    public class InventorySystem : AGameSystem, IDataBlockHandler<InventoryDataBlock>
    {
        public int money => m_money;
        public Dictionary<Item, int> items => m_items;

        private int m_money = 0;
        private Dictionary<Item, int> m_items = new();

        public int GetItemCount(Item item)
        {
            if (items.TryGetValue(item, out int count))
            {
                return count;
            }

            return 0;
        }

        public void AddMoney(int value)
        {
            if (value > 0)
            {
                m_money += value;
                GameManager.NotificationSystem.moneyAdded.Invoke(value);
            }
        }

        public void RemoveMoney(int value)
        {
            if (value > 0)
            {
                m_money = math.max(money - value, 0);
                GameManager.NotificationSystem.moneyRemoved.Invoke(value);
            }
        }

        public bool TryRemoveMoney(int value)
        {
            if (HasSufficientFunds(value))
            {
                RemoveMoney(value);
                return true;
            }

            return false;
        }

        public bool HasSufficientFunds(int value)
        {
            return value <= money;
        }

        public bool HasItemInBag(Item item, int quantity = 1)
        {
            return items.ContainsKey(item) && items[item] >= quantity;
        }

        public Equipment GetEquipment(EEquipmentType type)
        {
            if (GameManager.Player.equipments.ContainsKey(type))
            {
                return GameManager.Player.equipments[type];
            }

            return null;
        }

        public EEquipmentOperationResult TryEquip(Equipment equipment)
        {
            Debug.Assert(equipment, "Cannot equip a null equipment");

            EEquipmentOperationResult result = GameManager.Player.TryEquip(equipment, out Equipment previousEquipment);

            if (result == EEquipmentOperationResult.Valid)
            {
                RemoveFromBag(equipment, 1, EItemTransferType.Equipment);

                if (previousEquipment)
                {
                    AddToBag(previousEquipment, 1, EItemTransferType.Equipment);
                }
            }

            return result;
        }

        public EEquipmentOperationResult TryUnequip(EEquipmentType type)
        {
            EEquipmentOperationResult result = GameManager.Player.TryUnequip(type, out Equipment previousEquipment);

            if (result == EEquipmentOperationResult.Valid)
            {
                if (previousEquipment != null)
                {
                    AddToBag(previousEquipment, 1, EItemTransferType.Equipment);
                }
            }

            return result;
        }

        public void AddToBag(Item item, int quantity = 1, EItemTransferType source = EItemTransferType.Unknown)
        {
            if (!items.ContainsKey(item))
            {
                items.Add(item, quantity);
            }
            else
            {
                items[item] += quantity;
            }

            GameManager.NotificationSystem.itemAdded.Invoke(item, quantity, source);
        }

        public bool RemoveFromBag(Item item, int quantity = 1, EItemTransferType transferType = EItemTransferType.Unknown)
        {
            bool success = false;

            if (items.ContainsKey(item))
            {
                if (quantity >= items[item])
                {
                    items.Remove(item);
                }
                else
                {
                    items[item] -= quantity;
                }

                success = true;
            }

            GameManager.NotificationSystem.itemRemoved.Invoke(item, quantity, transferType);

            return success;
        }

        public void LoadDataBlock(InventoryDataBlock block)
        {
            m_money = block.money;
            m_items = block.items.ToDictionary(kvp => GameManager.Database.LoadFromReference(kvp.Key), kvp => kvp.Value);
        }

        public InventoryDataBlock CreateDataBlock()
        {
            return new InventoryDataBlock
            {
                money = m_money,
                items = new SerializableDictionary<DatabaseEntryReference<Item>, int>(m_items.ToDictionary(kvp => GameManager.Database.CreateReference(kvp.Key), kvp => kvp.Value))
            };
        }
    }
}
