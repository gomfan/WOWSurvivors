using System;
using System.Threading.Tasks;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class CloseMenus : ICommand
    {
        public Task Execute()
        {
            GameManager.NotificationSystem.closeAllMenusRequested.Invoke();
            return Task.CompletedTask;
        }
    }
}
