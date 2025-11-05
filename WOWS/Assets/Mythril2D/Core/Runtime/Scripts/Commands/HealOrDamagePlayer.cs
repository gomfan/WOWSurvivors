using System;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class HealOrDamagePlayer : ICommand
    {
        [SerializeField] private EAction m_action = EAction.Add;
        [SerializeField][Min(0)] private int m_amount = 0;

        public Task Execute()
        {
            switch (m_action)
            {
                case EAction.Add:
                    GameManager.Player.Heal(m_amount);
                    break;

                case EAction.Remove:
                    GameManager.Player.Damage(new DamageOutputDescriptor
                    {
                        source = new UnknownDamageSource(),
                        damage = math.abs(m_amount),
                        flags = EDamageFlag.None,
                        type = EDamageType.None,
                    });
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
