using System;
using System.Threading.Tasks;
using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class CommandInteraction : IInteraction
    {
        [SerializeReference, SubclassSelector] private ICommand m_command = null;

        public async Task<bool> TryExecute(CharacterBase source, IInteractionTarget target)
        {
            if (m_command != null)
            {
                await m_command.Execute();
                return true;
            }

            return false;
        }
    }
}
