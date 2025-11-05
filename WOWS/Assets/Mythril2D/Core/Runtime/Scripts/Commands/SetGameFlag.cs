using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class SetGameFlag : ICommand
    {
        [SerializeField] private string m_flagID = string.Empty;
        [SerializeField] private bool m_state = true;

        public Task Execute()
        {
            GameManager.GameFlagSystem.Set(m_flagID, m_state);
            return Task.CompletedTask;
        }
    }
}
