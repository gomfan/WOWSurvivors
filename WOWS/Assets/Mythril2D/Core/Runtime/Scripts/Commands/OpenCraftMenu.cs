using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class OpenCraftMenu : ICommand
    {
        [SerializeField] private CraftingStation m_craftingStation = null;

        public async Task Execute()
        {
            Debug.Assert(m_craftingStation != null, "Missing CraftingStation reference!");
            var taskCompletionSource = new TaskCompletionSource<bool>();
            GameManager.NotificationSystem.craftRequested.Invoke(m_craftingStation, taskCompletionSource);
            await taskCompletionSource.Task;
        }
    }
}
