using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ItemEquipOrUnequip : IItemEffect
    {
        [SerializeField] private AudioClipResolver m_equipSound = null;
        [SerializeField] private AudioClipResolver m_unEquipSound = null;

        private void OnOperationSuccess(EOperationType operationType)
        {
            AudioClipResolver audioClip =
                operationType == EOperationType.Equip ?
                m_equipSound :
                m_unEquipSound;

            if (audioClip)
            {
                GameManager.NotificationSystem.audioPlaybackRequested.Invoke(audioClip);
            }
        }

        private string GetReason(EEquipmentOperationResult operationResult)
        {
            Debug.Assert(operationResult != EEquipmentOperationResult.Valid, "This method should only be called when the operation result is not valid");

            switch (operationResult)
            {
                case EEquipmentOperationResult.NotEnoughHealth: return "this could kill you!";
                case EEquipmentOperationResult.NotEnoughMana: return "this could leave you with less than no <mana>!";
            }

            return "trust me!";
        }

        private Task OnOperationFailure(EOperationType operationType, EEquipmentOperationResult operationResult)
        {
            string operation = operationType.ToString().ToLower();
            string reason = GetReason(operationResult);
            return GameManager.DialogueSystem.Main.PlayNow($"You can't {operation} this item, {reason}");
        }

        public async Task<bool> TryUse(Item item, CharacterBase target, EItemLocation location)
        {
            Debug.Assert(item is Equipment, "");

            Equipment equipment = (Equipment)item;

            EOperationType operationType =
                location == EItemLocation.Bag ?
                EOperationType.Equip :
                EOperationType.Unequip;

            EEquipmentOperationResult operationResult =
                operationType == EOperationType.Equip ?
                GameManager.InventorySystem.TryEquip(equipment) :
                GameManager.InventorySystem.TryUnequip(equipment.type);

            if (operationResult == EEquipmentOperationResult.Valid)
            {
                OnOperationSuccess(operationType);
            }
            else
            {
                await OnOperationFailure(operationType, operationResult);
            }

            return true;
        }
    }
}
