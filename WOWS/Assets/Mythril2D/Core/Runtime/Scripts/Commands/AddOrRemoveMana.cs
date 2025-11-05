using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class AddOrRemoveMana : ICommand
    {
        [SerializeField] private EAction m_action = EAction.Add;
        [SerializeField][Min(0)] private int m_amount = 0;

        public Task Execute()
        {
            switch (m_action)
            {
                case EAction.Add:
                    GameManager.Player.RecoverMana(m_amount);
                    break;

                case EAction.Remove:
                    GameManager.Player.ConsumeMana(m_amount);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
