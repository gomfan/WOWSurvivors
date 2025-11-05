using System;
using System.Threading.Tasks;
using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    public enum ECommandListExecutionMode
    {
        Sequential,
        Parallel
    }

    [Serializable]
    public class ExecuteCommandList : ICommand
    {
        [SerializeField] private ECommandListExecutionMode m_executionMode = ECommandListExecutionMode.Sequential;
        [SerializeField] private EActionFlags m_disabledActions = EActionFlags.None;
        [SerializeReference, SubclassSelector] private ICommand[] m_commands = null;

        private async Task ExecuteSequential()
        {
            foreach (ICommand command in m_commands)
            {
                await command.Execute();
            }
        }

        private async Task ExecuteParallel()
        {
            Task[] tasks = new Task[m_commands.Length];

            for (int i = 0; i < m_commands.Length; i++)
            {
                tasks[i] = m_commands[i].Execute();
            }

            await Task.WhenAll(tasks);
        }

        public async Task Execute()
        {
            GameManager.Player.DisableActions(m_disabledActions);

            if (m_executionMode == ECommandListExecutionMode.Sequential)
            {
                await ExecuteSequential();
            }
            else
            {
                await ExecuteParallel();
            }

            GameManager.Player.EnableActions(m_disabledActions);
        }
    }
}
