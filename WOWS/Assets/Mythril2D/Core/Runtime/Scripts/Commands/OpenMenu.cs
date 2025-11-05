using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Gyvr.Mythril2D
{
    public enum EMenu
    {
        Pause,
        Character,
        Abilities,
        Inventory,
        Journal,
        Save,
        Settings,
        Death
    }

    [Serializable]
    public class OpenMenu : ICommand
    {
        [SerializeField] private EMenu m_menuToOpen;

        public UnityEvent<TaskCompletionSource<bool>> GetEventForSelectedMenu()
        {
            switch (m_menuToOpen)
            {
                case EMenu.Pause: return GameManager.NotificationSystem.gameMenuRequested;
                case EMenu.Character: return GameManager.NotificationSystem.statsRequested;
                case EMenu.Abilities: return GameManager.NotificationSystem.spellBookRequested;
                case EMenu.Inventory: return GameManager.NotificationSystem.inventoryRequested;
                case EMenu.Journal: return GameManager.NotificationSystem.journalRequested;
                case EMenu.Save: return GameManager.NotificationSystem.saveMenuRequested;
                case EMenu.Settings: return GameManager.NotificationSystem.settingsRequested;
                case EMenu.Death: return GameManager.NotificationSystem.deathScreenRequested;
            }

            Debug.Assert(false, "Invalid menu type!");

            return null;
        }

        public async Task Execute()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            GetEventForSelectedMenu().Invoke(taskCompletionSource);
            await taskCompletionSource.Task;
        }
    }
}
