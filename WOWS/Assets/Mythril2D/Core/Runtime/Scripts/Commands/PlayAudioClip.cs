using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class PlayAudioClip : ICommand
    {
        [SerializeField] private AudioClipResolver m_audioClip = null;

        public Task Execute()
        {
            GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_audioClip);
            return Task.CompletedTask;
        }
    }
}
