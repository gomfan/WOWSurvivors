using System;
using System.Threading.Tasks;
using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ApplyEffectsToPlayer : ICommand
    {
        [SerializeReference, SubclassSelector] private IEffect[] m_effects;

        public Task Execute()
        {
            EffectDispatcher.Apply(null, new[] { GameManager.Player }, m_effects);
            return Task.CompletedTask;
        }
    }
}
