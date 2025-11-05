using System;
using System.Threading.Tasks;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class RevivePlayer : ICommand
    {
        public Task Execute()
        {
            GameManager.Player.Revive();
            return Task.CompletedTask;
        }
    }
}
