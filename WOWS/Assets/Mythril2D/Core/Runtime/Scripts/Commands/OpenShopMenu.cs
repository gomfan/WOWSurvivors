using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class OpenShopMenu : ICommand
    {
        [SerializeField] private Shop m_shop = null;

        public async Task Execute()
        {
            Debug.Assert(m_shop != null, "Missing Shop reference!");
            var taskCompletionSource = new TaskCompletionSource<bool>();
            GameManager.NotificationSystem.shopRequested.Invoke(m_shop, taskCompletionSource);
            await taskCompletionSource.Task;
        }
    }
}
