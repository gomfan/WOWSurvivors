using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class PlayDialogueLine : ICommand
    {
        [SerializeField] private string m_speaker = string.Empty;
        [TextArea][SerializeField] private string m_line = string.Empty;

        public async Task Execute()
        {
            await GameManager.DialogueSystem.Main.PlayNow(new DialogueTree(new DialogueNode(StringFormatter.Format(m_line), m_speaker)));
        }
    }
}
