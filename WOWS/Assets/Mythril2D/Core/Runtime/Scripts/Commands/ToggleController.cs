using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ToggleController : ICommand
    {
        [SerializeField] private CharacterBase m_character = null;
        [SerializeField] private bool m_enabled = true;

        public Task Execute()
        {
            if (m_enabled)
            {
                m_character.controller?.Start();
            }
            else
            {
                m_character.controller?.Stop();
            }

            return Task.CompletedTask;
        }
    }
}
