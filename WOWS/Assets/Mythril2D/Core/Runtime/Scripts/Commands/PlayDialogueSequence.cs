using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class PlayDialogueSequence : ICommand
    {
        [SerializeField] private DialogueSequence m_dialogueSequence = null;
        [SerializeField] private string m_speaker = string.Empty;

        public Task Execute()
        {
            return GameManager.DialogueSystem.Main.PlayNow(m_dialogueSequence.ToDialogueTree(m_speaker));
        }
    }
}
