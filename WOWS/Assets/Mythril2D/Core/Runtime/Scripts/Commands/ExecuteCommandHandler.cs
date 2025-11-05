using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ExecuteCommandHandler : ICommand
    {
        [SerializeField] private CommandHandler m_commandHandler = null;

        public Task Execute() => m_commandHandler?.Execute();
    }
}
