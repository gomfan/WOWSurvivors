using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class AddExperience : ICommand
    {
        [SerializeField] private int m_experience;

        public Task Execute()
        {
            GameManager.Player.AddExperience(m_experience);
            return Task.CompletedTask;
        }
    }
}
