using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class DialogueInteraction : IInteraction
    {
        [SerializeField] private DialogueSequence m_sequence = null;

        public async Task<bool> TryExecute(CharacterBase source, IInteractionTarget target)
        {
            if (m_sequence != null)
            {
                await target.Say(m_sequence);
                return true;
            }

            return false;
        }
    }
}
