using System;
using System.Threading.Tasks;
using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class SaveCheckpoint : ICommand
    {
        [SerializeReference, SubclassSelector] private ICheckpoint m_checkpoint;

        public Task Execute()
        {
            GameManager.MapSystem.SaveCheckpoint(m_checkpoint);
            return Task.CompletedTask;
        }
    }
}
