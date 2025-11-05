using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class DestroyEntity : ICommand
    {
        [SerializeField] private Entity m_toDestroy = null;

        public Task Execute()
        {
            m_toDestroy.Destroy();
            return Task.CompletedTask;
        }
    }
}
