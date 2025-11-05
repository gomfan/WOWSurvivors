using System;
using System.Threading.Tasks;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class RespawnPlayer : ICommand
    {
        public Task Execute()
        {
            GameManager.MapSystem.RespawnPlayer();
            return Task.CompletedTask;
        }
    }
}
